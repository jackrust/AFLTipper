using System;
using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService.API;
using AustralianRulesFootball;

namespace AFLStatisticsService
{
    class Program
    {
        const double Tolerance = 0.01;
        private const int StartingYear = 2000;

        static void Main()
        {
            var db = new MongoDb();
            var loop = true;
            const string options = "[B]oth update & extend, [E]xtend, [U]pdate, [Q]uit, [?]Options";
            Console.WriteLine("AFL Statistics Service");
            while (loop)
            {
                Console.Write("\n>");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("D"):
                        Console.WriteLine("Deleting Season");
                        //db.DeleteSeason(2020);
                        break;

                    case ("B"):
                        Console.WriteLine("Both update & extend");
                        UpdateMatches(db);
                        ExtendMatches(db);
                        //TODO: Reinstate once Mongo works
                        //Console.WriteLine("Updating Players");
                        //UpdatePlayers(db, updateFromYear);
                        break;

                    case ("E"):
                        Console.WriteLine("Extending Matches");
                        ExtendMatches(db);
                        //TODO: Reinstate once Mongo works
                        //Console.WriteLine("Updating Players");
                        //UpdatePlayers(db, updateFromYear);
                        break;

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


        private static RoundUid GetLastCompletedRoundUid(List<Season> seasons)
        {
            //What have I got?
            seasons = seasons.OrderBy(s => s.Year).ToList();

            var lastCompletedRound =
                seasons.SelectMany(s => s.Rounds)
                    .Where(
                        r => r.Matches.All(m => m.HomeScore().Total() > Tolerance
                                                || m.AwayScore().Total() > Tolerance)
                        && r.Matches.Count > 0)
                    .OrderByDescending(r => r.Matches[0].Date)
                    .FirstOrDefault();

            var roundUid = new RoundUid();
            roundUid.Year = lastCompletedRound == null ? StartingYear : lastCompletedRound.Year;
            roundUid.Number = lastCompletedRound == null ? 0 : lastCompletedRound.Number;
            return roundUid;
        }

        private static void ExtendMatches(MongoDb db)
        {
            var seasons = db.GetSeasons().ToList();
            var roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Extending from " + roundUid.Year + ", " + roundUid.Number);

            //add any new matches between last match and now
            var api = new FootyWireApi();
            seasons = api.UpdateFrom(seasons, roundUid.Year, roundUid.Number + 1).ToList();
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasons(seasons);
        } 

        private static void UpdateMatches(MongoDb db)
        {
            //What have I got?
            var seasons = db.GetSeasons().ToList();
            var roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Updating Matches " + roundUid.Year + ", " + roundUid.Number);

            //add any new matches between last match and now
            var api = new WikipediaApi();
            seasons = api.UpdateFrom(seasons, roundUid.Year, roundUid.Number + 1).ToList();
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasons(seasons);
        }
    }
}
