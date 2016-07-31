using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AustralianRulesFootball;
using Match = AustralianRulesFootball.Match;

namespace AFLStatisticsService.API
{
    internal class FinalSirenApi
    {
        private const string Website = "http://finalsiren.com/";
        private const string Results = Website + "Results.asp";
        private const int ResultTableIndex = 2;

        public static int GetNumRounds(int year)
        {
            var numRounds = 0;
            var parameters = new Dictionary<string, string> { { "SeasonID", year.ToString() }, { "Round", "1-1" } };
            var page = WebsiteAPI.GetPage(Results, parameters);
            var roundList = WebsiteAPI.SplitOn(page, "<select", "</select")[1];

            var r = new Regex("([R])([1-9])+");
            var m = r.Match(roundList);
            while (m.Success)
            {
                var round = Int32.Parse(m.ToString().Remove(0,1));
                if(round > numRounds)
                    numRounds = round;
                m = m.NextMatch();
            }
            return numRounds;
        }

        public static Round GetRoundResults(int year, int roundNo)
        {
            var parameters = new Dictionary<string, string> {{"SeasonID", year.ToString()}, {"Round", roundNo + "-1"}};
            var page = WebsiteAPI.GetPage(Results, parameters);
            var table = WebsiteAPI.SplitOn(page, "<table", "</table")[0];
            var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr", 4);
            var matches = new List<Match>();

            for (var i = ResultTableIndex; i < rows.Count; i++)
            {
                var details = WebsiteAPI.SplitOn(rows[i], "<td", "</td", 4);
                matches.Add( new Match(
                    Util.GetTeamByName(ExtractFromXml(details[0])), 
                    new Score(details[1]),
                    new Score(details[2]) - new Score(details[1]),
                    new Score(details[3]) - new Score(details[2]),
                    new Score(details[4]) - new Score(details[3]),
                    Util.GetTeamByName(ExtractFromXml(details[7])),
                    new Score(details[8]),
                    new Score(details[9]) - new Score(details[8]),
                    new Score(details[10]) - new Score(details[9]),
                    new Score(details[11]) - new Score(details[10]),
                    Util.GetGroundByName(ExtractFromXml(details[13])),
                    Util.StringToDate(details[14] + " " + year.ToString())
                    ));
            }
            return new Round(Convert.ToInt32(year), roundNo, matches);
        }

        private static string ExtractFromXml(string str)
        {
            var sFlag= str.IndexOf(">", StringComparison.Ordinal);
            var eFlag = str.IndexOf("<", sFlag, StringComparison.Ordinal);
            if (sFlag > -1 && eFlag > -1)
                return str.Substring(sFlag + 1, eFlag - (sFlag + 1));
            return "";
        }
    }
}
