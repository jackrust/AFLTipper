using System.Collections.Generic;
using System.Linq;
using AFLTippingAPI.Controllers;
using AFLTippingAPI.Logic;
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

        // POST: api/Tipper
        [HttpPost]
        public void Post([FromBody] string value)
        {
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
