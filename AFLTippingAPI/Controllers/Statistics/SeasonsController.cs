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
    public class SeasonsController : ApiController
    {
        const double Tolerance = 0.01;
        private const int StartingYear = 2000;

        // GET api/statistics/seasons
        public string Get()
        {
            return GetSeasonInFormat(false, 0, "short");
        }

        // GET api/statistics/seasons
        public string Get([FromUri] string format)
        {
            return GetSeasonInFormat(false, 0, format);
        }

        // GET api/statistics/seasons/5
        public string Get(int id)
        {
            return GetSeasonInFormat(true, id, "short");
        }

        // GET api/statistics/seasons/5
        public string Get(int id, [FromUri] string format)
        {
            return GetSeasonInFormat(true, id, format);
        }

        private string GetSeasonInFormat(bool filter, int  year, string format)
        {
            string output;
            switch (format.ToLower())
            {
                case "short":
                    output = GetSeasonShortForm(filter, year);
                    break;
                case "long":
                    output = GetSeasonLongForm(filter, year);
                    break;
                default:
                    output = GetSeasonShortForm(filter, year);
                    break;
            }
            return output;
        }

        private string GetSeasonShortForm(bool filter, int year)
        {
            var db = new MongoDb();
            var seasons = db.GetSeasons().Where(s => (!filter || s.Year == year));
            var seasonsJson = seasons.Select(s =>
                $"{{{{Year:{s.Year}}}, {{Rounds:{s.Rounds.Count(r => !r.Matches.Any(m => m.HomeScore().Total() < 0.01 && m.AwayScore().Total() < 0.01))}}}}}");
            var output = seasonsJson.Aggregate("{", (current, seasonJson) => current + (seasonJson + ",")).TrimEnd(',')+"}";
            return output + "}";
        }

        private string GetSeasonLongForm(bool filter, int year)
        {
            var db = new MongoDb();
            var seasons = db.GetSeasons().Where(s => (!filter || s.Year == year)).ToList();
            var xmlList = seasons.Select(s => s.Stringify());
            //var json = Json.Encode(seasons);


            var xml = "<seasons>";
            foreach (var x in xmlList)
            {
                xml += "<season>" + x + "</season>";
            }
                      xml += "</seasons>";
            return xml;
        }


        // POST api/statistics/seasons
        public void Post([FromBody]string value)
        {
            UpdateSeasonsByMethod(value);
        }

        private void UpdateSeasonsByMethod(string value)
        {
            if (value == "")
            {
                var db = new MongoDb();
                UpdateAllSeasons(db);
            }
            else
            {
                List<Season> seasons = Json.Decode(value);
                var db = new MongoDb();
                db.UpdateSeasons(seasons);
            }

        }

        // PUT api/statistics/seasons/5
        public void Put(int id, [FromBody]object value)
        {
            UpdateSeasonByMethod(id, value.ToString());
        }

        private void UpdateSeasonByMethod(int year, string value)
        {
            if (value == "")
            {
                var db = new MongoDb();
                UpdateSeason(db, year);
            }
            else
            {
                var ss = Stringy.SplitOn(Stringy.SplitOn(value, "seasons")[0], "season");
                var season = ss.Select(Season.Objectify).ToList().First();
                if(season.Year != year)
                    return;
                //Season season = Json.Decode(value);
                var db = new MongoDb();
                db.UpdateSeasons(new List<Season>{season});
            }

        }
        private static void UpdateAllSeasons(MongoDb db)
        {
            //What have I got?
            var seasons = db.GetSeasons().ToList();
            seasons = seasons.OrderBy(s => s.Year).ToList();

            var lastCompletedRound =
                seasons.SelectMany(s => s.Rounds)
                    .Where(
                        r => r.Matches.All(m => m.HomeScore().Total() > Tolerance
                                                || m.AwayScore().Total() > Tolerance))
                    .OrderByDescending(r => r.Matches[0].Date)
                    .FirstOrDefault();
            var year = lastCompletedRound?.Year ?? StartingYear;
            var number = lastCompletedRound?.Number ?? 0;
            //add any new matches between last match and now
            seasons = UpdateFrom(db, seasons, year, number + 1);
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasons(seasons);
        }

        private static void UpdateSeason(MongoDb db, int year)
        {
            //What have I got?
            var seasons = db.GetSeasons().ToList();
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
            db.UpdateSeasons(seasons);
        }

        // DELETE api/statistics/seasons/5
        public void Delete(int id)
        {
            var db = new MongoDb();
            db.DeleteSeason(id);
        }
    }
}
