using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AustralianRulesFootball;
using ScreenScraper;
using Match = AustralianRulesFootball.Match;

namespace AFLStatisticsService.API
{
    internal class AFLAPI : AflStatisticsApi
    {
        private const string Website = "http://www.afl.com.au/";
        private const string Results = Website + "/fixture";
        private const int ResultTableIndex = 1;

        public override int GetNumRounds(int year)
        {
            var numRounds = 0;
            var parameters = new Dictionary<string, string>();
            var page = WebsiteAPI.GetPage(Results, parameters);
            var roundList = WebsiteAPI.SplitOn(page, "<select", "</select", "name=\"roundId\"")[0];

            var r = new Regex("(Round) ([0-9])+");
            var m = r.Match(roundList);
            while (m.Success)
            {
                var round = Int32.Parse(m.ToString().Remove(0, 6));
                if (round > numRounds)
                    numRounds = round;
                m = m.NextMatch();
            }
            return numRounds;
        }

        public override Round GetRoundResults(int year, int roundNo)
        {
            var isFinal = numHomeandAwayRounds[year] > roundNo;
            var roundString = roundNo < 10 ? "0" + roundNo : "" + roundNo;
            var parameters = new Dictionary<string, string> { { "roundId", "CD_R" + year + "014" + roundString } };
            var page = WebsiteAPI.GetPage(Results, parameters);
            var table = WebsiteAPI.SplitOn(page, "<table", "</table", "class=\"fancy-zebra fixture\"")[0];
            var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr", 4);
            var dateReg = new Regex("(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday), (January|February|March|April|May|June|July|August|September|October|November|December) ([0-9])+");
            var matches = new List<Match>();
            var dateHold = "";

            for (var i = ResultTableIndex; i < rows.Count; i++)
            {
                var dateMatch = dateReg.Match(rows[i]);
                if (dateMatch.Success) dateHold = dateMatch.ToString();
                if (!string.IsNullOrEmpty(dateHold))
                {
                    var details = WebsiteAPI.SplitOn(rows[i], "<td", "</td", 4);
                    if (details.Count > 1)
                    {
                        throw new Exception("AFLAPI doesn't load match statistics");

                        //Teams
                        var teams = WebsiteAPI.SplitOn(details[0], "<span class=\"team\"", "</span", 19);
                        var home = teams[0].TrimEnd('v').TrimEnd(' ');
                        var away = teams[1];

                        //Time
                        var ground = WebsiteAPI.SplitOn(details[1], "<a", "</a", "class=\"venue\"", 40)[0];
                        
                        //Time
                        var time = WebsiteAPI.SplitOn(details[1], "<span class=\"time\"", "</span", 19)[0];


                        matches.Add(new Match(
                            Util.GetTeamByName(home),
                            new Score(),
                            new Score(),
                            new Score(),
                            new Score(),
                            Util.GetTeamByName(away),
                            new Score(),
                            new Score(),
                            new Score(),
                            new Score(),
                            Util.GetGroundByName(ground),
                            Util.StringToDate(dateHold + " " + time + " " + year.ToString())
                            ));
                    }
                }
            }
            return new Round(Convert.ToInt32(year), roundNo, isFinal, matches);
        }

        public override List<Player> GetAllPlayers(int year)
        {
            throw new NotImplementedException();
        }

        public List<Player> GetAllPlayers()
        {
            return new List<Player>();
        }
    }
}
