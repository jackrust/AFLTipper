using System.Web.Http;
using API.Logic;

namespace API.Controllers.Statistics
{
    public class SeasonsController : ApiController
    {
        // GET api/statistics/seasons
        public string Get()
        {
            return SeasonLogic.GetSeasonInFormat(false, 0, "short");
        }

        // GET api/statistics/seasons
        public string Get([FromUri] string format)
        {
            return SeasonLogic.GetSeasonInFormat(false, 0, format);
        }

        // GET api/statistics/seasons/5
        public string Get(int id)
        {
            return SeasonLogic.GetSeasonInFormat(true, id, "short");
        }

        // GET api/statistics/seasons/5
        public string Get(int id, [FromUri] string format)
        {
            return SeasonLogic.GetSeasonInFormat(true, id, format);
        }

        // POST api/statistics/seasons
        public void Post([FromBody]string value)
        {
            SeasonLogic.UpdateSeasonsByMethod(value);
        }

        // PUT api/statistics/seasons/5
        public void Put(int id, [FromBody]object value)
        {
            SeasonLogic.UpdateSeasonByMethod(id, value.ToString());
        }

        // DELETE api/statistics/seasons/5
        public void Delete(int id)
        {
            //var db = new MongoDb();
            //db.DeleteSeason(id);
        }
    }
}
