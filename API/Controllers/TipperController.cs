using System.Collections.Generic;
using System.Linq;
using API.Controllers.Statistics;
using API.Logic;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TipperController : ControllerBase
    {
        // GET: api/Tipper
        [HttpGet]
        public IEnumerable<string> Get()
        {

            var predictions = NetworkLogic.JustTipFullSeason();
            var simplePredictions = SimplePrediction.Convert(predictions);
            var json = simplePredictions.Select(JsonConvert.SerializeObject);

            return json;
        }

        // GET: api/Tipper/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([System.Web.Http.FromBody]string value)
        {
            //Update Seasons
            SeasonLogic.Update();
            //Update Data Interpretation
            var interpretedDataController = new InterpretedDataController();
            interpretedDataController.Update();

        }

        // PUT: api/Tipper/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
