using System;
using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService.API;
using AustralianRulesFootball;
using Utilities;

namespace AFLStatisticsService
{
    class Program
    {
        const double Tolerance = 0.01;

        static void Main(string[] args)
        {
            var db = new MongoDB();
            //What have I got?
            var seasons = db.ReadDocument() ?? new List<Season>();

            var lastCompletedRound =
                seasons.SelectMany(s => s.Rounds)
                    .Where(
                        r => r.Matches.All(m => m.HomeScore().Total() > Tolerance 
                                             || m.AwayScore().Total() > Tolerance))
                    .OrderByDescending(r => r.Matches[0].Date)
                    .FirstOrDefault();
            var year = lastCompletedRound == null? 2007 : lastCompletedRound.Year;
            var number = lastCompletedRound == null ? 1 : lastCompletedRound.Number;
            //add any new matches between last match and now

            seasons = UpdateFrom(seasons, year, number+1);
            
            //update db
            db.UpdateDocument("season", seasons.Select(s => s.Bsonify()).ToList());
        }

        private static List<Season> UpdateFrom(List<Season> seasons, int year, int number)
        {
            Console.WriteLine(year + ", " + number);
            var successful = true;
            while (successful)
            {
                try
                {
                    var numRounds = FinalSirenApi.GetNumRounds(year);
                    for (var i = number; i <= numRounds; i++)
                    {
                        var round = FinalSirenApi.GetRoundResults(year, i);
                        if (round.Matches.All(m => m.HomeScore().Total() > Tolerance
                                                   || m.AwayScore().Total() > Tolerance))
                        {
                            if (seasons.First(s => s.Year == year).Rounds.Count >= i)
                            {
                                seasons.First(s => s.Year == year).Rounds[i - 1] = round;
                            }
                            else
                            {
                                seasons.First(s => s.Year == year).Rounds.Add(round);
                            }
                        }
                        else
                        {
                            successful = false;
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

        public static League Load()
        {
            //TODO: work these out from the API
            const int startingYear = 2007;

            var league = new League();

            
            for (var j = startingYear; j <= 2016; j++)
            {
                var numRounds = FinalSirenApi.GetNumRounds(j);
                var rounds = new List<Round> { };
                for (var i = 1; i <= numRounds; i++)
                {
                    rounds.Add(FinalSirenApi.GetRoundResults(j, i));
                }
                league.Seasons.Add(new Season(j, rounds));
            }
            

            /*var oddsMatches = OddsCSV.LoadMatchOddsList().Where(x => !x.IsFinal).OrderBy(x => x.Date).ToList();
            var matches = (from season in league.Seasons from round in season.Rounds from match in round.Matches select match).ToList();

            foreach (var oddsMatch in oddsMatches)
            {
                var match = matches.First(x => Equals(x.Home, oddsMatch.Home) && Equals(x.Away, oddsMatch.Away) && Datey.Approximates(x.Date, oddsMatch.Date));
                if (match != null)
                {
                    match.HomeOdds = oddsMatch.HomeOdds;
                    match.AwayOdds = oddsMatch.AwayOdds;
                }
                else
                {
                    Console.WriteLine("Error: League.Load");
                }
            }*/

            return league;
        }

        public static League Objectify(string str)
        {
            var ss = Stringy.SplitOn(Stringy.SplitOn(str, "seasons")[0], "season");
            var seasons = ss.Select(Season.Objectify).ToList();
            return new League(seasons);
        }
    }
}
