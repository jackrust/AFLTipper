using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AFLStatisticsService
{
    public class MongoDb
    {
        //C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe;
        public static readonly string LocalConnectionString = "mongodb://localhost/Seasons";
        public static readonly string LocalDatabaseName = "afl";
        public static readonly string RemoteDatabaseName = "appharbor_qzz6l02h";
        private readonly string _mongoDbConnectionString;
        private readonly string _databaseName;

        public MongoDb()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Bias)))
                BsonClassMap.RegisterClassMap<Bias>();
            _mongoDbConnectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                                      ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                                      LocalConnectionString;
            _databaseName = _mongoDbConnectionString == LocalConnectionString ? LocalDatabaseName : RemoteDatabaseName;
        }

        #region ReleaseMongo

        public IEnumerable<Season> GetSeasons()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName).GetCollection<Season>("Seasons");
            var nullFilter = new FilterDefinitionBuilder<Season>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public void UpdateSeasons(List<Season> seasons)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName).GetCollection<Season>("Seasons");

            try
            {
                session.StartTransaction();
                foreach (var s in seasons)
                {
                    var nullFilter = new FilterDefinitionBuilder<Season>().Empty;
                    var results = databaseSeasons.Find(nullFilter).ToList();

                    if (results.Any(dbs => dbs.Year == s.Year))
                    {
                        var filter = new BsonDocument("Year", s.Year);
                        databaseSeasons.DeleteOne(filter);
                        databaseSeasons.InsertOne(s);
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
                throw;
            }
        }

        #endregion

        public void DeleteSeason(int year)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName).GetCollection<Season>("Seasons");

            try
            {
                session.StartTransaction();

                var filter = new BsonDocument("Year", year);
                databaseSeasons.DeleteOne(filter);

                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                throw;
            }
        }

        public IEnumerable<Network> GetNetworks()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName).GetCollection<Network>("Networks");
            var nullFilter = new FilterDefinitionBuilder<Network>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public void UpdateNetworks(List<Network> networks)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseNetworks = session.Client.GetDatabase(_databaseName).GetCollection<Network>("Networks");

            try
            {
                session.StartTransaction();
                foreach (var n in networks)
                {
                    var nullFilter = new FilterDefinitionBuilder<Network>().Empty;
                    var results = databaseNetworks.Find(nullFilter).ToList();

                    if (results.Any(dbs => dbs.Id == n.Id))
                    {
                        databaseNetworks.DeleteOne(dbn => dbn.Id == n.Id);
                        databaseNetworks.InsertOne(n);
                    }
                    else
                    {
                        databaseNetworks.InsertOne(n);
                    }
                }

                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                throw;
            }
        }

        public void DeleteNetwork(string id)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseNetworks = session.Client.GetDatabase(_databaseName).GetCollection<Network>("Networks");

            try
            {
                session.StartTransaction();

                databaseNetworks.DeleteOne(dbn => dbn.Id == id);
                session.CommitTransaction();
            }
            catch (Exception e)
            {
                session.AbortTransaction();
                throw;
            }
        }//First, broken network = 1c9e05b2-e765-4009-a3b9-a573fdb4765a
    }
}
