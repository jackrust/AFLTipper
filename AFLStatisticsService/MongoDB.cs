using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using AustralianRulesFootball;
using MongoDB.Driver;
using MongoDB.Bson;

namespace AFLStatisticsService
{
    public class MongoDb
    {
        //C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe;
        private static readonly string LocalConnectionstring = "mongodb://localhost/Seasons";
        private static readonly string LocalDatabaseName = "afl";
        private static readonly string RemoteDatabaseName = "appharbor_qzz6l02h";
        private string MongoDbConnectionString;
        private string DatabaseName;

        public MongoDb()
        {
            MongoDbConnectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                LocalConnectionstring;
            DatabaseName = MongoDbConnectionString == LocalConnectionstring ? LocalDatabaseName : RemoteDatabaseName;
        }

        #region ReleaseMongo
        //TODO: Replace List with IEnumerable - it will prevent enumaration early and speed shit up
        public List<Season> ReadSeasonDocument()
        {
            var client = new MongoClient(MongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(DatabaseName).GetCollection<Season>("Seasons");
            var nullFilter = new FilterDefinitionBuilder<Season>().Empty;
            var results = databaseSeasons.Find<Season>(nullFilter).ToList();
            return results;
        }

        //TODO: this is getting silly, better to just create dedicated actions (insert, update, delete) rather than this catch all
        public void UpdateSeasonDocument(List<Season> seasons)
        {
            var client = new MongoClient(MongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(DatabaseName).GetCollection<Season>("Seasons");

            try
            {
                session.StartTransaction();
                foreach (var s in seasons)
                {
                    var nullFilter = new FilterDefinitionBuilder<Season>().Empty;
                    var results = databaseSeasons.Find<Season>(nullFilter).ToList();

                    if (results.Any(dbs => dbs.Year == s.Year))
                    {
                        var filter = new BsonDocument("Year", s.Year.ToString());
                        databaseSeasons.ReplaceOne(filter, s);
                    }
                    else
                    {
                        databaseSeasons.InsertOne(s);
                    }
                }
                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                return;
            }
        }
        #endregion
    }
}
