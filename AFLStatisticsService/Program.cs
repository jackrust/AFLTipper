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
        private const int StartingYear = 2000;

        static void Main()
        {
            var db = new MongoDb();
            var updateFromYear = DateTime.Now.Year;
            var loop = true;
            const string options = "[U]pdate, [Q]uit, [?]Options";
            Console.WriteLine("AFL Statistics Service");
            while (loop)
            {
                Console.Write("\n>");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("U"):
                        Console.WriteLine("Updating Matches");
                        UpdateMatches(db);
                        //TODO: Reinstate once Mongo works
                        //Console.WriteLine("Updating Players");
                        //UpdatePlayers(db, updateFromYear);
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    case ("?"):
                        Console.WriteLine(options);
                        break;
                }
            }
        }

        //TODO: Might be less page hits to update by match (once we have their starting data)
        /*private static void UpdatePlayers(MongoDb db, int updateFromYear)
        {
            Console.WriteLine("Beginning UpdatePlayers");
            var year = DateTime.Now.Year;

            //What have I got?
            List<Player> players = db.ReadPlayerDocument() ?? new List<Player>();
            var api = new FinalSirenApi();

            //Get histry for each player
            for (var i = year; i >= updateFromYear; i--)
            {
                var newplayers = api.GetPlayers(i);

                foreach (var n in newplayers.Where(n => players.All(p => p.FinalSirenPlayerId != n.FinalSirenPlayerId)))
                {
                    players.Add(n);
                }

                foreach (var p in players)
                {
                    if (p.History.Count == 0)
                        p.History = api.GetPlayerMatchHistory(p.FinalSirenPlayerId);
                }
            }

            //update db
            db.UpdatePlayerDocument(players.Select(p => p.Bsonify()).ToList());
        }*/

        private static void UpdateMatches(MongoDb db)
        {
            Console.WriteLine("Beginning UpdateMatches");
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
            var year = lastCompletedRound == null ? StartingYear : lastCompletedRound.Year;
            var number = lastCompletedRound == null ? 0 : lastCompletedRound.Number;
            //add any new matches between last match and now
            seasons = UpdateFrom(seasons, year, number + 1).ToList();
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasons(seasons);
        }

        private static List<Season> UpdateFrom(List<Season> seasons, int year, int number)
        {
            Console.WriteLine("Beginning UpdateFrom");
            if(seasons.Count == 1)
                seasons = new List<Season>();
            var api = new FinalSirenApi();//new AFLAPI();//
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

        /*public static League Objectify(string str)
        {
            var ss = Stringy.SplitOn(Stringy.SplitOn(str, "seasons")[0], "season");
            var seasons = ss.Select(Season.Objectify).ToList();
            return new League(seasons);
        }*/
    }
}
