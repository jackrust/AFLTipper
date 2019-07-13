using System;
using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService;
using AFLStatisticsService.API;
using AustralianRulesFootball;

namespace AFLTippingAPI.Logic
{
    public class StatisticsLogic
    {
        private const double Tolerance = 0.01;
        private const int StartingYear = 2000;

        public static List<Season> LoadSeasons()
        {
            var db = new MongoDb();
            var seasons = db.GetSeasons() ?? new List<Season>();
            return seasons.OrderBy(s => s.Year).ToList();
        }

        public static void UpdateAllSeasons(MongoDb db)
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

        public static void UpdateSeason(MongoDb db, int year)
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
    }
}