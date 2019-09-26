using System;
using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService;
using AFLStatisticsService.API;
using AustralianRulesFootball;
using Newtonsoft.Json;
using Utilities;

namespace API.Logic
{
    public class SeasonLogic
    {
        const double Tolerance = 0.01;
        private const int StartingYear = 2000;

        public static string GetSeasonInFormat(bool filter, int year, string format)
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

        public static string GetSeasonShortForm(bool filter, int year)
        {
            var db = new MongoDb();
            var seasons = db.GetSeasons().Where(s => (!filter || s.Year == year));
            var seasonsJson = seasons.Select(s =>
                $"{{{{Year:{s.Year}}}, {{Rounds:{s.Rounds.Count(r => !r.Matches.Any(m => m.HomeScore().Total() < 0.01 && m.AwayScore().Total() < 0.01))}}}}}");
            var output = seasonsJson.Aggregate("{", (current, seasonJson) => current + (seasonJson + ",")).TrimEnd(',') + "}";
            return output + "}";
        }

        public static string GetSeasonLongForm(bool filter, int year)
        {
            var db = new MongoDb();
            var seasons = db.GetSeasons().Where(s => (!filter || s.Year == year)).ToList();
            var xmlList = seasons.Select(s => s.Stringify());

            var xml = "<seasons>";
            foreach (var x in xmlList)
            {
                xml += "<season>" + x + "</season>";
            }
            xml += "</seasons>";
            return xml;
        }

        public static void Update()
        {
            UpdateSeasonsByMethod("");
        }

        public static void UpdateSeasonsByMethod(string value)
        {
            var db = new MongoDb();
            if (value == "")
            {
                UpdateAllSeasons();
            }
            else
            {
                List<Season> seasons = JsonConvert.DeserializeObject<List<Season>>(value);
                db.UpdateSeasons(seasons);
            }

        }

        public static void UpdateAllSeasons()
        {
            var db = new MongoDb();
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
            seasons = UpdateFrom(seasons, year, number + 1);
        }

        public static void UpdateSeason(int year)
        {
            var db = new MongoDb();
            //What have I got?
            var seasons = db.GetSeasons().ToList();
            seasons = seasons.OrderBy(s => s.Year).ToList();

            var number = 0;
            //add any new matches between last match and now
            Update(seasons, year, number + 1);
        }

        public static List<Season> UpdateFrom(List<Season> seasons, int year, int number)
        {
            if (seasons.Count == 1)
                seasons = new List<Season>();
            Console.WriteLine(year + ", " + number);
            var successful = true;
            while (successful)
            {
                try
                {
                    Update(seasons, year, number);
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

        public static void Update(List<Season> seasons, int year, int number)
        {
            var db = new MongoDb();
            var api = new FinalSirenApi();
            var newDataStillComingTroughFromApi = true;

            var numRounds = api.GetNumRounds(year);//TODO:speed up - maybe store in code for now, in DB later
            for (var i = number; (i <= numRounds && newDataStillComingTroughFromApi); i++)
            {
                var round = api.GetRoundResults(year, i);
                if (round.Matches.TrueForAll(m => m.HomeScore().Total() < 0.1 && m.AwayScore().Total() < 0.1) && seasons.First(s => s.Year == year).Rounds.Count == numRounds)
                {
                    newDataStillComingTroughFromApi = false;
                }

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
                    db.UpdateSeasons(seasons.Where(s => s.Year >= year).ToList());
                }
            }
            db.UpdateSeasons(seasons.Where(s => s.Year >= year).ToList());
        }

        public static void UpdateSeasonByMethod(int year, string value)
        {
            var db = new MongoDb();
            if (value == "")
            {
                UpdateSeason(year);
            }
            else
            {
                var ss = Stringy.SplitOn(Stringy.SplitOn(value, "seasons")[0], "season");
                var season = ss.Select(Season.Objectify).ToList().First();
                if (season.Year != year)
                    return;
                //Season season = Json.Decode(value);
                db.UpdateSeasons(new List<Season> {season});
            }
        }
    }
}