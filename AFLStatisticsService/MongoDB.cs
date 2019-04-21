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
        private string MongoDbConnectionString;
        private const string MongoDbExeLocal = @"C:\Program Files\MongoDB\Server\3.2\bin\mongod.exe";//@"C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe"
        private const string DatabaseName = "afl";

        public MongoDb()
        {
            MongoDbConnectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                "mongodb://localhost/Seasons";
        }

        #region Combined

        //TODO: Can we just load player data into season data?
        /*public List<Season> ReadSeasonAndPlayerDocuments()
        {
            var database = LoadMongoDatabase();
            //Load seasons
            var seasonsCollection = database.GetCollection<BsonDocument>("season");
            var seasonsDocument = seasonsCollection.Find(_ => true).ToListAsync();
            seasonsDocument.Wait();
            var seasons = ObjectifySeason(seasonsDocument.Result);

            //Load player matches
            var playersCollection = database.GetCollection<BsonDocument>("player");
            var playersDocument = playersCollection.Find(_ => true).ToListAsync();
            playersDocument.Wait();
            var players = ObjectifySeason(playersDocument.Result);
            //Append player data
            foreach (var s in seasons)
            {
                foreach (var r in s.Rounds)
                {
                    foreach (var m in r.Matches)
                    {
                        
                    }
                }
            }
        } */
        #endregion

        #region ReleaseMongo
        //TODO: Replace List with IEnumerable - it will prevent enumaration early and speed shit up
        public List<Season> ReadSeasonDocument()
        {
            var client = new MongoClient(MongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase("afl").GetCollection<Season>("Seasons");
            var nullFilter = new FilterDefinitionBuilder<Season>().Empty;
            var results = databaseSeasons.Find<Season>(nullFilter).ToList();
            return results;
        }

        //TODO: this is getting silly, better to just create dedicated actions (insert, update, delete) rather than this catch all
        public void UpdateSeasonDocument(List<Season> seasons)
        {
            //var database = LoadMongoDatabase();
            //var collection = database.GetCollection<Season>("Seasons");
            var client = new MongoClient(MongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase("afl").GetCollection<Season>("Seasons");

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
                        //collection.Update(Query.EQ("Year", s.Year.ToString()), Update.Replace(s), UpdateFlags.Upsert);
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

            var clientz = new MongoClient(MongoDbConnectionString);
            var sessionz = client.StartSession();
            var databaseSeasonsz = session.Client.GetDatabase("afl").GetCollection<Season>("Seasons");
            var nullFilterz = new FilterDefinitionBuilder<Season>().Empty;
            var resultsz = databaseSeasons.Find<Season>(nullFilterz).ToList();
        }

        #endregion


        #region Season
        /*public void InsertSeasonDocumentz(List<BsonDocument> seasons)
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("season");

            var document = new BsonArray();
            foreach (var season in seasons)
            {
                document.Add(season);
            }

            collection.InsertMany(seasons);
        }*/

        /*public List<Season> ReadSeasonDocument()
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("season");
            var documents = collection.Find(_ => true).ToListAsync();
            documents.Wait();
            return ObjectifySeason(documents.Result);
        }*/
        /*
        public void UpdateSeasonDocument(List<BsonDocument> seasons)
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("season");

            foreach (var s in seasons)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("year", s.GetValue("year").AsInt32);

                if (collection.Count(filter) > 0)
                {
                    collection.ReplaceOne(filter, s);
                }
                else
                {
                    collection.InsertOne(s);
                }
            }
        }*/

        /*private List<Season> ObjectifySeason(List<BsonDocument> documents)
        {
            var seasons = new List<Season>();
            foreach (var d in documents)
            {
                try
                {
                    var season = Season.Objectify(d);
                    seasons.Add(season);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return seasons;
        }*/
        #endregion

        #region Player
        /*public List<Player> ReadPlayerDocument()
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("player");
            var documents = collection.Find(_ => true).ToListAsync();
            documents.Wait();
            return ObjectifyPlayer(documents.Result);
        }

        public void UpdatePlayerDocument(List<BsonDocument> players)
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("player");

            foreach (var p in players)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("finalSirenPlayerId", p.GetValue("finalSirenPlayerId").AsInt32);

                if (collection.Count(filter) > 0)
                {
                    collection.ReplaceOne(filter, p);
                }
                else
                {
                    collection.InsertOne(p);
                }
            }
        }

        private List<Player> ObjectifyPlayer(List<BsonDocument> documents)
        {
            var players = new List<Player>();
            foreach (var d in documents)
            {
                try
                {
                    var player = Player.Objectify(d);
                    players.Add(player);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return players;
        }*/
        #endregion


        /*private static IMongoDatabase LoadMongoDatabase()
        {
            if (!IsProcessOpen("mongod"))
            {
                Process.Start(MongoDbExe);
            }

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(DatabaseName);
            return database;
        }*/

        public static bool IsProcessOpen(string name)
        {
            foreach (var process in Process.GetProcesses())
            {

                if (process.ProcessName.Contains(name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
