using System;
using System.Collections.Generic;
using System.Diagnostics;
using AustralianRulesFootball;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AFLStatisticsService
{
    public class MongoDb
    {
        private const string MongoDbExe = @"C:\Program Files\MongoDB\Server\3.2\bin\mongod.exe";
        private const string DatabaseName = "afl";

        #region Season
        public void InsertSeasonDocument(List<BsonDocument> seasons)
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("season");

            var document = new BsonArray();
            foreach (var season in seasons)
            {
                document.Add(season);
            }

            collection.InsertMany(seasons);
        }

        public List<Season> ReadSeasonDocument()
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("season");
            var documents = collection.Find(_ => true).ToListAsync();
            documents.Wait();
            return ObjectifySeason(documents.Result);
        }

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
            
        }

        private List<Season> ObjectifySeason(List<BsonDocument> documents)
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
        }
        #endregion

        #region Player
        public List<Player> ReadPlayerDocument()
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
        }
        #endregion


        private static IMongoDatabase LoadMongoDatabase()
        {
            if (!IsProcessOpen("mongod"))
            {
                Process.Start(MongoDbExe);
            }

            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(DatabaseName);
            return database;
        }

        public static bool IsProcessOpen(string name)
        {
            //here we're going to get a list of all running processes on
            //the computer
            foreach (Process clsProcess in Process.GetProcesses())
            {
                //now we're going to see if any of the running processes
                //match the currently running processes. Be sure to not
                //add the .exe to the name you provide, i.e: NOTEPAD,
                //not NOTEPAD.EXE or false is always returned even if
                //notepad is running.
                //Remember, if you have the process running more than once, 
                //say IE open 4 times the loop thr way it is now will close all 4,
                //if you want it to just close the first one it finds
                //then add a return; after the Kill
                if (clsProcess.ProcessName.Contains(name))
                {
                    //if the process is found to be running then we
                    //return a true
                    return true;
                }
            }
            //otherwise we return a false
            return false;
        }
    }
}
