using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using AustralianRulesFootball;
using System.Linq;
using ScreenScraper;
using Match = AustralianRulesFootball.Match;
using HtmlAgilityPack;
using System.Data;

namespace AFLStatisticsService.API
{
    internal class WikipediaApi : AflStatisticsApi
    {
        private const string Website = "https://en.wikipedia.org/wiki/";

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

        public override Round GetRoundResultsHomeAndAway(int year, int roundNo)
        {
            var isFinal = numHomeandAwayRounds[year] < roundNo;
            var link = Website + year + "_AFL_season";

            var web = new HtmlWeb();
            var doc = web.Load(link);

            var nodes = doc.DocumentNode.SelectNodes("//table[tbody/tr/th[text()='Round " + roundNo + "\n' or contains(text(),'Round " + roundNo + " (')]]/tbody/tr[not(@style)]")
                .Where(n => n.InnerText.Contains("\nvs.\n") || n.InnerText.Contains("\ndef.\n") || n.InnerText.Contains("\ndef. by\n")).ToList();


            var dateReg = new Regex("(Sunday|Monday|Tuesday|Wednesday|Thursday|Friday|Saturday), ([0-9])+ (January|February|March|April|May|June|July|August|September|October|November|December) \\(([0-9])+:([0-9])+.*pm\\)");
            var matches = new List<Match>();
            var dateHold = "";

            foreach (var node in nodes)
            {
                var date = node.SelectSingleNode("td").InnerText;
                var dateMatch = dateReg.Match(date);
                if (dateMatch.Success) 
                    dateHold = dateMatch.ToString();
                var teams = node.SelectSingleNode("td[2]");

                //Teams
                var home = node.SelectSingleNode("td[2]").SelectSingleNode("a").InnerText;
                var away = node.SelectSingleNode("td[4]").SelectSingleNode("a").InnerText;

                //Ground
                var ground = node.SelectSingleNode("td[5]").SelectSingleNode("a").InnerText;


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

        public override List<Round> GetRoundResultsFinals(int year)
        {
            //throw new NotImplementedException();
            return new List<Round>();
        }
    }
}
