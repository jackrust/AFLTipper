using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Http;
using AFLStatisticsService;
using ArtificialNeuralNetwork;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace AFLTippingAPI.Controllers.NeuralNetwork
{
    public class NetworkController : ApiController
    {
        private MongoDb db;

        public NetworkController()
        {
            db = new MongoDb();
        }
        // GET api/statistics/seasons
        public string Get()
        {
            return GetNetwork("");
        }

        // GET api/statistics/seasons/5
        public string Get(string id)
        {
            return GetNetwork(id);
        }



        private string GetNetwork(string id)
        {
            
            var networks = db.GetNetworks().Where(n => (id.Length == 0 || n.Id == id)).ToList();
            var bsonStrings = new List<string>();
            foreach (var network in networks)
            {
                var bson = network.ToBson();
                string str = Convert.ToBase64String(bson);
                //TODO: test, delete
                byte[] bytesz = Convert.FromBase64String(str);
                var networkz = BsonSerializer.Deserialize<Network>(bytesz);
                bsonStrings.Add(str);
            }

            return bsonStrings.ToJson();
        }


        // POST api/statistics/seasons
        public void Post([FromBody]object value)
        {
            var networks = ConvertToNetworkList(value);

            var db = new MongoDb();
            db.UpdateNetworks(networks);
        }

        // PUT api/statistics/seasons/5
        public void Put(string id, [FromBody]object value)
        {
            var networks = ConvertToNetworkList(value);

            db.UpdateNetworks(networks.Where(n => n.Id == id).ToList());
        }

        private List<Network> ConvertToNetworkList(object value)
        {
            var json = (string)value;
            var bsonStrings = Json.Decode(json);
            var networks = new List<Network>();
            foreach (var bsonString in bsonStrings)
            {
                byte[] bytes = Convert.FromBase64String(bsonString);
                var network = BsonSerializer.Deserialize<Network>(bytes);
                networks.Add(network);
            }

            return networks;
        }

        // DELETE api/statistics/seasons/5
        public void Delete(string id)
        {
            //delete
            //db.DeleteNetwork(id);

            //clean
            /*var networks = db.GetNetworks();
            foreach (var network in networks.Where(n => n.Id != "b5345230-2ff1-48d3-8682-a9a8ac035de6"))
            {
                db.DeleteNetwork(network.Id);
            }*/
        }
    }
}
