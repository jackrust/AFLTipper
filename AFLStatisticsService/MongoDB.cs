using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace AFLStatisticsService
{
    public class MongoDB
    {
        public async void InsertDocument()
        {
            string inputFileName = "json/json.txt";
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("foo");
            IMongoCollection<BsonDocument> collection; // initialize to the collection to write to.
            collection = database.GetCollection<BsonDocument>("bar");

            using (var streamReader = new StreamReader(inputFileName))
            {
                string line;
                while ((line = await streamReader.ReadLineAsync()) != null)
                {
                    using (var jsonReader = new JsonReader(line))
                    {
                        var context = BsonDeserializationContext.CreateRoot(jsonReader);
                        var document = collection.DocumentSerializer.Deserialize(context);
                        await collection.InsertOneAsync(document);
                    }
                }
            }
        }

        public async void ReadDocument()
        {
            var client = new MongoClient("mongodb://localhost:27017");
            var database = client.GetDatabase("foo");
            IMongoCollection<BsonDocument> collection; // initialize to the collection to write to.
            collection = database.GetCollection<BsonDocument>("bar");
            var filter = Builders<BsonDocument>.Filter.Eq("x", 1.0);
            await collection.Find(filter).ForEachAsync(d => Console.WriteLine(d));
        }
    }
}
