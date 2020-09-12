using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AustralianRulesFootball;
using HtmlAgilityPack;
using ScreenScraper;
using Match = AustralianRulesFootball.Match;

namespace AFLStatisticsService.API
{
    internal class FootyWireApi : AflStatisticsApi
    {
        private const string Website = "https://www.footywire.com";
        private const int ResultTableIndex = 1;

        public override int GetNumRounds(int year)
        {
            //https://www.footywire.com/afl/footy/ft_match_list?year=2019
            var numRounds = 0;
            var parameters = new Dictionary<string, string>();
            var results = Website + "/afl/footy/ft_match_list?year=" + year;
            var page = WebsiteAPI.GetPage(results, parameters);

            var r = new Regex("(Round) ([0-9])+");
            var m = r.Match(page);
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
            var rounds = numHomeandAwayRounds[year];
            var link = Website + "/afl/footy/ft_match_list?year=" + year;

            var web = new HtmlWeb();
            var doc = web.Load(link);

            var nodes = doc.DocumentNode.SelectNodes("//tr[td/a[@name='round_"+roundNo+"']]/following::tr");

            var matchNodes = nodes.Skip(1).TakeWhile(n => n.InnerText != "" && !n.InnerText.Contains("BYE"));


            var dateReg = new Regex("(Sun|Mon|Tue|Wed|Thu|Fri|Sat) ([0-9])+ (Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec) ?(([0-9])+:([0-9])+.*pm)?");
            var matches = new List<Match>();
            var dateHold = "";

            foreach (var node in matchNodes)
            {
                var date = node.SelectSingleNode("td").InnerText;
                var dateMatch = dateReg.Match(date);
                if (dateMatch.Success) dateHold = dateMatch.ToString();

                //Teams
                var home = node.SelectSingleNode("td[2]").SelectSingleNode("a[1]").InnerText;
                var away = node.SelectSingleNode("td[2]").SelectSingleNode("a[2]").InnerText;

                //Ground
                var ground = node.SelectSingleNode("td[3]").InnerText;

                var match = new Match(
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
                    Util.StringToDate(dateHold + " " + year)
                );

                var midTag = node.SelectSingleNode("td[5]").SelectSingleNode("a");
                    
                if (midTag != null)
                {
                    var mid = midTag.GetAttributes("href").First().Value.Replace("ft_match_statistics?mid=", "");
                    ExtendMatch(match, mid);
                }

                matches.Add(match);
            }
            return new Round(Convert.ToInt32(year), roundNo, isFinal, matches);
        }

        public void ExtendMatch(Match match, string mid)
        {
            var NUM_QUARTERS = 4;
            var QUARTER_SKIP = 1;
            var matchParameters = new Dictionary<string, string>();
            matchParameters.Add("mid", mid);
            var matchUrl = Website + "/afl/footy/ft_match_statistics";
            var matchPage = WebsiteAPI.GetPage(matchUrl, matchParameters);
            var scores = WebsiteAPI.SplitOn(matchPage, "id=\"matchscoretable", "/table>")[0];
            var rows = WebsiteAPI.SplitOn(scores, "<tr", "/tr>");
            
            for (int j = 1; j < rows.Count; j++)
            {
                var quarters = WebsiteAPI.SplitOn(rows[j], "<td", "\n");
                var team = Util.GetTeamByName(quarters[0].Split('>')[2].Split('<')[0]);

                var goals = new List<int>();
                var points = new List<int>();

                for (int i = QUARTER_SKIP; i < NUM_QUARTERS + QUARTER_SKIP; i++)
                {
                    var scoreparts = quarters[i].Split('>')[1].Split('.');
                    goals.Add(Int32.Parse(scoreparts[0])-goals.Sum());
                    points.Add(Int32.Parse(scoreparts[1]) - points.Sum());
                    if (match.Home.Equals(team))
                    {
                        
                        match.Quarters[i - QUARTER_SKIP].HomeScore =
                            new Score(goals[i - QUARTER_SKIP], points[i - QUARTER_SKIP]);
                    }
                    else
                    {
                        match.Quarters[i - QUARTER_SKIP].AwayScore =
                            new Score(goals[i - QUARTER_SKIP], points[i - QUARTER_SKIP]);
                    }
                }
            }

            var statsTable = WebsiteAPI.SplitOn(matchPage, "Head to Head", "/table>")[1];
            var statsRows = WebsiteAPI.SplitOn(statsTable, "<tr", "/tr>");
            var teams = WebsiteAPI.SplitOn(statsRows[0], "<td", "/td>");
            var leftTeam = Util.GetTeamByName(teams[1].Split('>')[1].Split('<')[0]);
            var rightTeam = Util.GetTeamByName(teams[3].Split('>')[1].Split('<')[0]);

            var kicks = "";
            var handballs = "";
            var disposals = "";
            var marks = "";
            var tackles = "";
            var hitouts = "";
            var freesfor = "";
            var freesagainst = "";
            var rushedbehinds = "";

            var clearences = "";
            var clangers = "";
            var rebound50 = "";
            var inside50 = "";
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
