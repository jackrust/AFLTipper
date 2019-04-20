using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using AFLStatisticsService;
using AFLStatisticsService.API;
using AustralianRulesFootball;

namespace AFLTippingAPI.Controllers
{
    public class StatisticsController : ApiController
    {
        const double Tolerance = 0.01;
        private const int StartingYear = 2000;

        // GET api/statistics
        public IEnumerable<string> Get()
        {
            var db = new MongoDb();
            var seasons = db.ReadSeasonDocument() ?? new List<Season>();
            seasons = seasons.OrderBy(s => s.Year).ToList();
            return seasons.Select(s => s.Year.ToString());
        }

        // GET api/statistics/5
        public string Get(int id)
        {
            var db = new MongoDb();
            var seasons = db.ReadSeasonDocument() ?? new List<Season>();
            return seasons.Any(s => s.Year == id)? "Exists" : "Does not exist";
        }

        // POST api/statistics
        public void Post([FromBody]string value)
        {
            var db = new MongoDb();
            UpdateAllSeasons(db);
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
            UpdateSeason(db, id);
        }

        // DELETE api/statistics/5
        public void Delete(int id)
        {
        }

        private static void UpdateAllSeasons(MongoDb db)
        {
            //What have I got?
            var seasons = db.ReadSeasonDocument() ?? new List<Season>();
            seasons = seasons.OrderBy(s => s.Year).ToList();

            var lastCompletedRound =
                seasons.SelectMany(s => s.Rounds)
                    .Where(
                        r => r.Matches.All(m => m.HomeScore().Total() > Tolerance
                                                || m.AwayScore().Total() > Tolerance))
                    .OrderByDescending(r => r.Matches[0].Date)
                    .FirstOrDefault();
            var year = lastCompletedRound == null ? StartingYear : lastCompletedRound.Year;
            var number = lastCompletedRound == null ? 0 : lastCompletedRound.Number;
            //add any new matches between last match and now
            seasons = UpdateFrom(db, seasons, year, number + 1);
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasonDocument(seasons);
        }

        private static void UpdateSeason(MongoDb db, int year)
        {
            //What have I got?
            var seasons = db.ReadSeasonDocument() ?? new List<Season>();
            seasons = seasons.OrderBy(s => s.Year).ToList();

            var number = 0;
            //add any new matches between last match and now
            Update(db, seasons, year, number + 1);
        }

        private static List<Season> UpdateFrom(MongoDb db, List<Season> seasons, int year, int number)
        {
            if (seasons.Count == 1)
                seasons = new List<Season>();
            Console.WriteLine(year + ", " + number);
            var successful = true;
            while (successful)
            {
                try
                {
                    Update(db, seasons, year, number);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    successful = false;
                }
                if (successful)
                {
                    //We're still getting fresh data so loop into next season:
                    year++;
                    number = 1;
                    seasons.Add(new Season(year, new List<Round>()));
                }
            }
            return seasons;
        }

        private static void Update(MongoDb db, List<Season> seasons, int year, int number)
        {

            var api = new FinalSirenApi();

            var numRounds = api.GetNumRounds(year);
            for (var i = number; i <= numRounds; i++)
            {
                var round = api.GetRoundResults(year, i);
                if (seasons.All(s => s.Year != year))
                {
                    seasons.Add(new Season(year, new List<Round>()));
                }

                if (seasons.First(s => s.Year == year).Rounds.Count >= i)
                {
                    seasons.First(s => s.Year == year).Rounds[i - 1] = round;
                }
                else
                {
                    seasons.First(s => s.Year == year).Rounds.Add(round);
                }
            }
            db.UpdateSeasonDocument(seasons);
        }
    }
}
