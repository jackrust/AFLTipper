using System;
using System.Collections.Generic;
using System.Diagnostics;
using AustralianRulesFootball;
using MongoDB.Bson;
using MongoDB.Driver;

namespace AFLStatisticsService
{
    public class MongoDB
    {
        private const string MongoDbExe = @"C:\Program Files\MongoDB\Server\3.2\bin\mongod.exe";
        private const string DatabaseName = "foo"; //"afl";

        public void InsertDocument(string collectionName, List<BsonDocument> seasons)
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("bar");

            var document = new BsonArray();
            foreach (var season in seasons)
            {
                document.Add(season);
            }

            collection.InsertMany(seasons);
        }

        public List<Season> ReadDocument()
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("bar");
            //var filter = Builders<BsonDocument>.Filter.Eq("year", "2014");
            var documents = collection.Find(_ => true).ToListAsync();
            documents.Wait();
            return Objectify(documents.Result);
        }

        public void UpdateDocument(string collectionName, List<BsonDocument> seasons)
        {
            var database = LoadMongoDatabase();
            var collection = database.GetCollection<BsonDocument>("bar");

            foreach (var s in seasons)
            {
                var filter = Builders<BsonDocument>.Filter.Eq("year", s.GetValue("year").AsInt32);
                collection.ReplaceOne(filter, s);
            }
            
        }

        private List<Season> Objectify(List<BsonDocument> documents)
        {
            var seasons = new List<Season>();
            foreach (var d in documents)
            {
                try
                {
                    var season = Season.Objectify(d);
                    seasons.Add(season);
                    Console.WriteLine("success: season " + season.Year);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            return seasons;
        }


        private static IMongoDatabase LoadMongoDatabase()
        {
            Process.Start(MongoDbExe);
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase(DatabaseName);
            return database;
        }


    }
}
