using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AustralianRulesFootball.DataAccess
{
    internal class AFLAPI
    {
        private const string Website = "http://www.afl.com.au/";
        private const string Results = Website + "/fixture";
        private const int ResultTableIndex = 1;

        public static int GetNumRounds(int year)
        {
            var numRounds = 0;
            var parameters = new Dictionary<string, string> {};
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

        public static Round GetRoundResults(int year, int roundNo)
        {
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
            return new Round(Convert.ToInt32(year), roundNo, matches);
        }

        private static string ExtractFromXml(string str)
        {
            var sFlag = str.IndexOf(">", StringComparison.Ordinal);
            var eFlag = str.IndexOf("<", sFlag, StringComparison.Ordinal);
            if (sFlag > -1 && eFlag > -1)
                return str.Substring(sFlag + 1, eFlag - (sFlag + 1));
            return "";
        }
    }
}
