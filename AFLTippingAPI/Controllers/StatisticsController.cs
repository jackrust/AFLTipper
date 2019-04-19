using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using AFLStatisticsService;
using AFLStatisticsService.API;
using AustralianRulesFootball;
using Utilities;

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
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            UpdateMatches(db);
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);
            return new List<string> { string.Format("elapsedTime: {0}", elapsedTime) };
        }

        // GET api/statistics/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/statistics
        public void Post([FromBody]string value)
        {
        }

        // PUT api/statistics/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/statistics/5
        public void Delete(int id)
        {
        }

        private static void UpdateMatches(MongoDb db)
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
            seasons = UpdateFrom(seasons, year, number + 1);
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasonDocument(seasons.Select(s => s.Bsonify()).ToList());
        }

        private static List<Season> UpdateFrom(List<Season> seasons, int year, int number)
        {
            if (seasons.Count == 1)
                seasons = new List<Season>();
            var api = new FinalSirenApi();
            Console.WriteLine(year + ", " + number);
            var successful = true;
            while (successful)
            {
                try
                {
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

        public static League Objectify(string str)
        {
            var ss = Stringy.SplitOn(Stringy.SplitOn(str, "seasons")[0], "season");
            var seasons = ss.Select(Season.Objectify).ToList();
            return new League(seasons);
        }
    }
}
