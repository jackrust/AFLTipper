using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Cricket;

namespace AFLStatisticsService
{
    public class MongoDb
    {
        //C:\Program Files\MongoDB\Server\3.6\bin\mongod.exe;
        public static readonly string LocalConnectionString = "mongodb://localhost/Seasons";
        public static readonly string LocalDatabaseName_AFL = "afl";
        public static readonly string LocalDatabaseName_BBL = "bbl";
        public static readonly string RemoteDatabaseName = "appharbor_qzz6l02h";
        private readonly string _mongoDbConnectionString;
        private readonly string _databaseName_AFL;
        private readonly string _databaseName_BBL;

        public MongoDb()
        {
            if (!BsonClassMap.IsClassMapRegistered(typeof(Bias)))
                BsonClassMap.RegisterClassMap<Bias>();
            _mongoDbConnectionString = ConfigurationManager.AppSettings.Get("MONGOHQ_URL") ??
                                      ConfigurationManager.AppSettings.Get("MONGOLAB_URI") ??
                                      LocalConnectionString;
            _databaseName_AFL = _mongoDbConnectionString == LocalConnectionString ? LocalDatabaseName_AFL : RemoteDatabaseName;
            _databaseName_BBL = _mongoDbConnectionString == LocalConnectionString ? LocalDatabaseName_AFL : RemoteDatabaseName;
        }

        #region Seasons
        public IEnumerable<Season> GetSeasons()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Season>("Seasons");
            var nullFilter = new FilterDefinitionBuilder<Season>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public IEnumerable<BBLSeason> GetBBLSeasons()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_BBL).GetCollection<BBLSeason>("BBLSeasons");
            var nullFilter = new FilterDefinitionBuilder<BBLSeason>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public IEnumerable<BBLSeason> GetWBBLSeasons()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_BBL).GetCollection<BBLSeason>("WBBLSeasons");
            var nullFilter = new FilterDefinitionBuilder<BBLSeason>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public void UpdateSeasons(List<Season> seasons)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Season>("Seasons");

            try
            {
                //session.StartTransaction();
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

                //session.CommitTransaction();
            }
            catch (Exception e)
            {
                //session.AbortTransaction();
                throw;
            }
        }

        public void UpdateBBLSeasons(List<BBLSeason> seasons)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_BBL).GetCollection<BBLSeason>("BBLSeasons");

            try
            {
                //session.StartTransaction();
                foreach (var s in seasons)
                {
                    var nullFilter = new FilterDefinitionBuilder<BBLSeason>().Empty;
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

                //session.CommitTransaction();
            }
            catch (Exception e)
            {
                //session.AbortTransaction();
                throw;
            }
        }

        public void UpdateWBBLSeasons(List<BBLSeason> seasons)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_BBL).GetCollection<BBLSeason>("WBBLSeasons");

            try
            {
                //session.StartTransaction();
                foreach (var s in seasons)
                {
                    var nullFilter = new FilterDefinitionBuilder<BBLSeason>().Empty;
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

                //session.CommitTransaction();
            }
            catch (Exception e)
            {
                //session.AbortTransaction();
                throw;
            }
        }

        public void DeleteSeason(int year)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Season>("Seasons");

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

        public void DeleteWBBLSeason(int year)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Season>("WBBLSeasons");

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
        #endregion

        #region Network
        public IEnumerable<Network> GetNetworks()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Network>("Networks");
            var nullFilter = new FilterDefinitionBuilder<Network>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public void UpdateNetworks(List<Network> networks)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseNetworks = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Network>("Networks");

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
            var databaseNetworks = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Network>("Networks");

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
        #endregion

        #region Data Interpretation
        public IEnumerable<Data> GetDataInterpretation()
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseSeasons = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Data>("DataInterpretation");
            var nullFilter = new FilterDefinitionBuilder<Data>().Empty;
            var results = databaseSeasons.Find(nullFilter).ToList();
            return results;
        }

        public void UpdateDataInterpretation(List<Data> datas)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseNetworks = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Data>("DataInterpretation");

            try
            {
                //session.StartTransaction();
                foreach (var d in datas)
                {
                    var nullFilter = new FilterDefinitionBuilder<Data>().Empty;
                    var results = databaseNetworks.Find(nullFilter).ToList();

                    if (results.Any(dbs => dbs.Id == d.Id))
                    {
                        databaseNetworks.DeleteOne(dbn => dbn.Id == d.Id);
                        databaseNetworks.InsertOne(d);
                    }
                    else
                    {
                        databaseNetworks.InsertOne(d);
                    }
                }

                //session.CommitTransaction();
            }
            catch (Exception e)
            {
                //session.AbortTransaction();
                throw;
            }
        }

        public void DeleteDataInterpretation(string id)
        {
            var client = new MongoClient(_mongoDbConnectionString);
            var session = client.StartSession();
            var databaseNetworks = session.Client.GetDatabase(_databaseName_AFL).GetCollection<Data>("DataInterpretation");

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
        #endregion
    }
}
