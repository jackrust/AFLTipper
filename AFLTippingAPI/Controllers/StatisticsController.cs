using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AFLStatisticsService;
using AFLTippingAPI.Logic;

namespace AFLTippingAPI.Controllers
{
    public class StatisticsController : ApiController
    {
        // GET api/statistics
        public IEnumerable<string> Get()
        {
            var seasons = StatisticsLogic.LoadSeasons();
            return seasons.Select(s => $"{s.Year.ToString()}, {s.Rounds.Count}");
        }

        // GET api/statistics/5
        public string Get(int id)
        {
            var seasons = StatisticsLogic.LoadSeasons();
            return seasons.Any(s => s.Year == id)? "Exists" : "Does not exist";
        }

        // POST api/statistics
        public void Post([FromBody]string value)
        {
            var db = new MongoDb();
            StatisticsLogic.UpdateAllSeasons(db);
        }

        // PUT api/statistics/5
        public void Put(int id, [FromBody]string value)
        {
            if (id < 2000)
            {
                return;
            }
            if (id > DateTime.Now.Year)
            {
                return;
            }
            var db = new MongoDb();
            StatisticsLogic.UpdateSeason(db, id);
        }

        // DELETE api/statistics/5
        public void Delete(int id)
        {
        }
    }
}
