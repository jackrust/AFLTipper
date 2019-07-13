using System.Web.Http;
using AFLTippingAPI.Controllers.Statistics;
using AFLTippingAPI.Logic;
using MongoDB.Bson;

namespace AFLTippingAPI.Controllers
{
    public class TipsController : ApiController
    {
        // GET api/values
        public string Get()
        {
            var predictions = NetworkLogic.JustTipFullSeason();
            var simplePredictions = SimplePrediction.Convert(predictions);
            return simplePredictions.ToJson();
        }
        

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            //Update Seasons
            SeasonLogic.Update();
            //Update Data Interpretation
            var interpretedDataController = new InterpretedDataController();
            interpretedDataController.Update();

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}