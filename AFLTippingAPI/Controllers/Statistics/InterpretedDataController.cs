using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Http;
using AFLStatisticsService;
using AFLTippingAPI.Helpers;
using ArtificialNeuralNetwork.DataManagement;
using Tipper;

namespace AFLTippingAPI.Controllers.Statistics
{
    public class InterpretedDataController : ApiController
    {
        private readonly MongoDb _db;
        private readonly int _batchSize = 500;

        public InterpretedDataController()
        {
            _db = new MongoDb();
        }

        // GET api/statistics/InterpretedData
        public string Get()
        {
            var db = new MongoDb();
            var data = db.GetDataInterpretation().First();
            return Json.Encode(data);
        }

        // GET api/statistics/InterpretedData/5
        public string Get(int id)
        {
            var data = _db.GetDataInterpretation().First();
            data.DataPoints = data.DataPoints.Skip(id).Take(_batchSize).ToList();
            return Json.Encode(data);
        }

        // POST api/statistics/InterpretedData
        public void Post([FromBody]string value)
        {
            Update();
        }

        public void Update()
        {
            var data = _db.GetDataInterpretation().FirstOrDefault() ?? new Data();
            AppendInterpretations(data);
            data.Id = Global.NeuralNetworkId;
            _db.UpdateDataInterpretation(new List<Data>() { data });
        }

        //Only updates forward in time, backward will need to be via put or something
        private void AppendInterpretations(Data data)
        {
            var seasons = _db.GetSeasons().ToList();
            //Do we need all this match data? Surely not.
            var lastDate = data.DataPoints.Select(dp => (Tuple<DateTime, string>) dp.Reference).Select(t => t.Item1).OrderByDescending(d => d).FirstOrDefault();
            var last = seasons.SelectMany(s => s.Rounds).OrderByDescending(r => r.Year).ThenByDescending(r => r.Number).First();
            var first = seasons.SelectMany(s => s.Rounds).Where(r => r.Matches.All(m => m.Date > lastDate))
                .OrderBy(r => r.Year).ThenBy(r => r.Number).FirstOrDefault();
            //If we can't find anything then we must be up to date
            if(first == null)
                return;
            var newData = Tipper.Tipper.GetMatchDataBetween(seasons, first.Year, first.Number, last.Year, last.Number, AFLDataInterpreter.Interpretations.BespokeLegacyInterpretation);
            //Check just in case
            if(newData.DataPoints.Select(p => ((Tuple<DateTime, string>)p.Reference).Item1).OrderBy(d => d).First() > lastDate)
                Console.WriteLine("Some new date is earlier that an old date");
            data.DataPoints.AddRange(newData.DataPoints);
        }

        // PUT api/statistics/InterpretedData/
        public void Put([FromBody]object value)
        {
            var str = value.ToString();
            var data = Json.Decode<Data>(str);
            AppendExisting(data);
            _db.UpdateDataInterpretation(new List<Data>(){ data });
        }

        private void AppendExisting(Data data)
        {
            //First, need to fix json misunderstanding the use of tuple
            for (var i = 0; i < data.DataPoints.Count; i++)
            {
                var dictionary = (Dictionary<string, object>)data.DataPoints[i].Reference;
                data.DataPoints[i].Reference = new Tuple<DateTime, string>((DateTime)dictionary["Item1"], (string)dictionary["Item2"]);
            }

            var dbData = _db.GetDataInterpretation().First();

            //filter to only new ones
            var filteredDataPoints = data.DataPoints.Where(dp => !dbData.DataPoints.Any(dbdp =>
                ((Tuple<DateTime, string>) dp.Reference).Item1 == ((Tuple<DateTime, string>) dbdp.Reference).Item1 &&
                ((Tuple<DateTime, string>) dp.Reference).Item2 == ((Tuple<DateTime, string>) dbdp.Reference).Item2));
            data.DataPoints = dbData.DataPoints;
            data.DataPoints.AddRange(filteredDataPoints);
            data.DataPoints = data.DataPoints.OrderBy(dp => ((Tuple<DateTime, string>) dp.Reference).Item1).ToList();
        }

        // PUT api/statistics/InterpretedData/5
        public void Put(int id, [FromBody]object value)
        {
           
        }


        // DELETE api/statistics/seasons/5
        public void Delete(string id)
        {
            _db.DeleteDataInterpretation(id);
        }
    }
}
