using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Helpers;
using System.Web.Http;
using AFLStatisticsService;
using AFLStatisticsService.API;
using AustralianRulesFootball;
using MongoDB.Bson;
using Utilities;

namespace AFLTippingAPI.Controllers.Statistics
{
    public class DataProjectionController : ApiController
    {

        // GET api/statistics/seasons
        public string Get()
        {
            return "";
        }

        // GET api/statistics/seasons/5
        public string Get(int id)
        {
            return "";
        }

        // POST api/statistics/seasons
        public void Post([FromBody]string value)
        {
            
        }

        // PUT api/statistics/seasons/5
        public void Put(int id, [FromBody]object value)
        {
           
        }


        // DELETE api/statistics/seasons/5
        public void Delete(int id)
        {
        }
    }
}
