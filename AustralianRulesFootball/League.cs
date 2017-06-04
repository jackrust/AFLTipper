using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AustralianRulesFootball.DataAccess;
using Utilities;

namespace AustralianRulesFootball
{
    public class League
    {
        public static int StartingYear = 2007;
        public List<Season> Seasons;

        public League() : this(new List<Season>())
        {
        }

        public League(List<Season> seasons)
        {
            Seasons = seasons;
        }

        public Season GetCurrentSeason()
        {
            return Seasons[Seasons.Count - 1];
        }

        public List<Season> GetSeasons(int fromYear, int toYear)
        {
            return Seasons.Where(s => s.Year >= fromYear && s.Year <= toYear).ToList();
        }

        public Season GetSeason(int year)
        {
            return Seasons.FirstOrDefault(s => s.Year == year);
        }

        public List<Round> GetRounds(int fromYear, int fromRound, int toYear, int toRound)
        {
            var rs = new List<Round>();
            if (fromRound < 0)
                fromYear--;
            foreach (var s in Seasons)
            {
                if (s.Year == fromYear && s.Year == toYear)
                    rs.AddRange(s.GetRounds(fromRound, toRound));
                else if (s.Year == fromYear)
                    rs.AddRange(s.GetRoundsFrom(fromRound));
                else if (s.Year > fromYear && s.Year < toYear)
                    rs.AddRange(s.Rounds);
                else if (s.Year == toYear)
                    rs.AddRange(s.GetRoundsTo(toRound));
            }
            return rs;
        }

        //TODO: Obselete: use MongoDB
        public static League Load()
        {
            var league = new League();
            const string fileName = "League/League.txt";

            //Get from XML if you can
            if (File.Exists(fileName))
            {
                var lines = File.ReadAllText(fileName);
                league = Objectify(lines);
                return league;
            }

            
            for (var j = StartingYear; j <= 2016; j++)
            {
                var numRounds = FinalSirenApi.GetNumRounds(j);
                var rounds = new List<Round> {};
                for (var i = 1; i <= numRounds; i++)
                {
                    try
                    {
                        rounds.Add(FinalSirenApi.GetRoundResults(j, i));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("EXCEPTION: " + e.Message);
                    }
                }
                league.Seasons.Add(new Season(j, rounds));
            }

            var oddsMatches = OddsCSV.LoadMatchOddsList().Where(x => !x.IsFinal).OrderBy(x => x.Date).ToList();
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
            }

            /*for (var j = 2016; j <= 2016; j++)
            {
                var numRounds = AFLAPI.GetNumRounds(j);
                var rounds = new List<Round> { };
                for (var i = 1; i <= numRounds; i++)
                {
                    rounds.Add(AFLAPI.GetRoundResults(j, i));
                }
                league.Seasons.Add(new Season(j, rounds));
            }*/

            
            //Write to file for next time
            var file = new StreamWriter(fileName);
            var s = league.Stringify();
            file.WriteLine(s);
            file.Close();
            
            return league;
        }

        #region IO

        public string Stringify()
        {
            var s = "";
            s += "<seasons>";
            foreach (var sn in Seasons)
            {
                s += "<season>";
                s += sn.Stringify();
                s += "</season>";
            }
            s += "</seasons>";
            return s;
        }

        public static League Objectify(string str)
        {
            var ss = Stringy.SplitOn(Stringy.SplitOn(str, "seasons")[0], "season");
            var seasons = ss.Select(Season.Objectify).ToList();
            return new League(seasons);
        }

        #endregion
    }
}
