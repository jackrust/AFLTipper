using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AustralianRulesFootball;
using System.Linq;
using ScreenScraper;
using Match = AustralianRulesFootball.Match;

namespace AFLStatisticsService.API
{
    internal class WikipediaApi : AflStatisticsApi
    {
        private const string Website = "https://en.wikipedia.org/wiki/";
        private const int ResultTableIndex = 0;

        public override int GetNumRounds(int year)
        {
            var numRounds = 0;
            var parameters = new Dictionary<string, string>();
            var results = Website + year + "_AFL_season";
            var page = WebsiteAPI.GetPage(results, parameters);

            var roundList = WebsiteAPI.SplitOn(page, "#Premiership_season", "</ul")[0];

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
            var isFinal = numHomeandAwayRounds[year] < roundNo;
            var parameters = new Dictionary<string, string>();
            var results = Website + year + "_AFL_season";
            var page = WebsiteAPI.GetPage(results, parameters);
            var section = WebsiteAPI.SplitOn(page, "<span class=\"mw-headline\" id=\"Premiership_season\">Premiership season</span>", "<h2><span id=\"Win.2Floss_table\"></span><span class=\"mw-headline\" id=\"Win/loss_table\">Win/loss table</span>")[0];
            var table = WebsiteAPI.SplitOn(section, "<span class=\"mw-headline\" id=\"Round_" + roundNo + "\">Round " + roundNo + "</span>", "</table")[0];

            var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr", 4);
            rows.RemoveAll(r => !r.Contains("vs.") && !r.Contains("def."));
            var dateReg = new Regex("(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday), ([0-9])+ (January|February|March|April|May|June|July|August|September|October|November|December) \\(([0-9])+:([0-9])+.*pm\\)");
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
                        var cleaned = "";
                        var homeouter = WebsiteAPI.SplitOn(details[1], ">", "/a>", 1)[0].Replace("<i>", "");
                        var home = WebsiteAPI.SplitOn(homeouter, ">", "<", 1)[0];
                        cleaned = details[3].Replace("style=\"font-weight: bold;\">", "").Replace("<i>", "");
                        var away = WebsiteAPI.SplitOn(cleaned, ">", "</a>", 1)[0];

                        //Ground
                        var groundouter = WebsiteAPI.SplitOn(details[4].Replace("<i>", ""), ">", "/a>", 1)[0];
                        var ground = WebsiteAPI.SplitOn(groundouter, ">", "<", 1)[0];


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
                            Util.StringToDate(dateHold.Replace("&#160;", "") + " " + year.ToString())
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
