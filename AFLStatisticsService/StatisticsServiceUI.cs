using System;
using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService.API;
using AustralianRulesFootball;

namespace AFLStatisticsService
{
    public class StatisticsServiceUI
    {
        const double Tolerance = 0.01;
        private const int StartingYear = 2000;

        public static void LoadMainLoop()
        {
            var db = new MongoDb();
            var loop = true;

            Console.WriteLine("AFL Statistics Service");
            while (loop)
            {
                Console.Write("\n>");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("A"):
                        AppendMatchStatistics(db);
                        break;

                    case ("B"):
                        Console.WriteLine("Updating BBL Matches");
                        ExtendBBLMatches(db);
                        break;

                    case ("D"):
                        Console.WriteLine("Deleting Season");
                        db.DeleteSeason(2021);
                        break;

                    case ("F"):
                        Console.WriteLine("Updating Matches (Footywire)");
                        UpdateMatchesFootyWire();
                        break;

                    case ("S"):
                        Console.WriteLine("Updating Matches (Final Siren)");
                        UpdateMatchesFinalSiren();
                        break;

                    case ("U"):
                        Console.WriteLine("Updating Matches (Legacy)");
                        UpdateMatchesFootyWire(db);
                        break;

                    case ("W"):
                        Console.WriteLine("Updating Matches (Wikipedia)");
                        UpdateMatchesWikipedia(db);
                        break;

                    case ("WB"):
                        Console.WriteLine("Updating WBBL Matches");
                        ExtendWBBLMatches(db);
                        break;

                    case ("Q"):
                        loop = false;
                        break;
                    case ("?"):
                        ListOptions();
                        break;
                }
            }

        }

        public static void ListOptions()
        {
            Console.WriteLine("[A]ppend match stats");
            Console.WriteLine("[B]BL data");
            Console.WriteLine("[D]elete season (manual)");
            Console.WriteLine("[F]ootyWire update AFL");
            Console.WriteLine("[U]pdate AFL");
            Console.WriteLine("[W]ikipedia update AFL");
            Console.WriteLine("[WB]BBL data");

            Console.WriteLine("[Q]uit");
            Console.WriteLine("[?] Show options");
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
            roundUid.IsFinal = lastCompletedRound == null ? false : lastCompletedRound.IsFinal;
            return roundUid;
        }

        private static void AppendMatchStatistics(MongoDb db)
        {
            var seasons = db.GetSeasons().ToList();
            //var roundUid = GetLastCompletedRoundUid(seasons);
            var roundUid = new RoundUid() { Year = StartingYear, Number = 0 };
            Console.WriteLine("Appending from " + roundUid.Year + ", " + roundUid.Number);

            //add any new matches between last match and now
            var api = new FootyWireApi();
            foreach (var round in seasons.SelectMany(s => s.Rounds))
            {
                if (round.Year >= 2010)
                {
                    if (round.Matches.Where(m => m.HomeStats is null || m.AwayStats is null || m.HomeStats.Clearances == 0 || m.AwayStats.Clearances == 0).Count() > 0)
                    {
                        api.AppendMatchStatisticstoResults(round);
                    }
                }
                else
                {
                    if (round.Matches.Where(m => m.HomeStats.Kicks == 0 || m.AwayStats.Kicks == 0).Count() > 0)
                        api.AppendMatchStatisticstoResults(round);
                }
            }
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasons(seasons);
        }

        public static void UpdateMatchesFootyWire(MongoDb db = null)
        {
            if(db == null)
                db = new MongoDb();

            //AFL
            var seasons = db.GetSeasons().ToList();
            var roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Extending from " + roundUid.Year + ", " + (roundUid.IsFinal ? "Finals week " : "Round ") + roundUid.Number);

            //add any new matches between last match and now
            var api = new FootyWireApi();

            seasons = api.UpdateFrom(seasons, roundUid).ToList();
            seasons.RemoveAll(s => s.Rounds.Count == 0);

            roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Last completed round: " + roundUid.Year + ", " + (roundUid.IsFinal ? "Finals week " : "Round ") + roundUid.Number);
            //update db
            db.UpdateSeasons(seasons);
        }

        public static void UpdateMatchesFinalSiren(MongoDb db = null)
        {
            if (db == null)
                db = new MongoDb();

            //AFL
            var seasons = db.GetSeasons().ToList();
            var roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Extending from " + roundUid.Year + ", " + (roundUid.IsFinal ? "Finals week " : "Round ") + roundUid.Number);

            //add any new matches between last match and now
            var api = new FinalSirenApi();

            seasons = api.UpdateFrom(seasons, roundUid).ToList();
            seasons.RemoveAll(s => s.Rounds.Count == 0);

            roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Last completed round: " + roundUid.Year + ", " + (roundUid.IsFinal ? "Finals week " : "Round ") + roundUid.Number);
            //update db
            db.UpdateSeasons(seasons);
        }

        private static void ExtendBBLMatches(MongoDb db)
        {
            //BBL
            var seasons = db.GetBBLSeasons().ToList();

            //var year = GetLastCompletedMatch(seasons);
            Console.WriteLine("Extending from " + 2011);

            //add any new matches between last match and now
            var api = new MyKhelAPI();
            seasons = api.UpdateFrom(2011, seasons).ToList();
            seasons.RemoveAll(s => s.Matches.Count == 0);
            //update db*/
            db.UpdateBBLSeasons(seasons);
        }

        private static void ExtendWBBLMatches(MongoDb db)
        {
            var seasons = db.GetWBBLSeasons().ToList();

            //var year = GetLastCompletedMatch(seasons);
            Console.WriteLine("Extending from " + 2015);

            //add any new matches between last match and now
            var api = new WikipediaWBBLAPI();
            seasons = api.UpdateFrom(2015, seasons).ToList();
            seasons.RemoveAll(s => s.Matches.Count == 0);
            //update db
            db.UpdateWBBLSeasons(seasons);
        }

        private static void UpdateMatchesWikipedia(MongoDb db)
        {
            //What have I got?
            var seasons = db.GetSeasons().ToList();
            var roundUid = GetLastCompletedRoundUid(seasons);
            Console.WriteLine("Updating Matches " + roundUid.Year + ", " + roundUid.Number);

            //add any new matches between last match and now
            var api = new WikipediaApi();
            seasons = api.UpdateFrom(seasons, roundUid).ToList();
            seasons.RemoveAll(s => s.Rounds.Count == 0);
            //update db
            db.UpdateSeasons(seasons);
        }
    }
}
