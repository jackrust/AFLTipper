using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
                    AppendMatchStatistics(match, mid);
                }

                matches.Add(match);
            }
            return new Round(Convert.ToInt32(year), roundNo, isFinal, matches);
        }

        public void ExtendMatch(Match match, string mid)
        {
            var NUM_QUARTERS = 4;
            var QUARTER_SKIP = 1;
            var link = Website + "/afl/footy/ft_match_statistics?mid=" + mid;
            var web = new HtmlWeb();
            var doc = web.Load(link);

            var nodes = doc.DocumentNode.SelectNodes("//table[@id='matchscoretable']//tr");
            
            for (int j = 1; j < nodes.Count(); j++)
            {
                var quarters = nodes[j].SelectNodes("td");
                
                var team = Util.GetTeamByName(quarters[0].InnerText);

                var goals = new List<int>();
                var points = new List<int>();

                for (int i = QUARTER_SKIP; i < NUM_QUARTERS + QUARTER_SKIP; i++)
                {
                
                    var scoreparts = quarters[i].InnerText.Split('.');
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
        }

        public void AppendMatchStatistics(Match match, string mid)
        {
            var link = Website + "/afl/footy/ft_match_statistics?mid=" + mid;
            var web = new HtmlWeb();
            var doc = web.Load(link);

            var nodes = doc.DocumentNode.SelectNodes("//table[tr/td[contains(text(),'Head to Head')]]//tr");

            if (match.HomeStats is null)
                match.HomeStats = new MatchStatistics();
            if (match.AwayStats is null)
                match.AwayStats = new MatchStatistics();

            foreach (var row in nodes.Skip(2))
            {
                if (row.SelectNodes("td")[0].InnerText == "" || row.SelectNodes("td")[0].InnerText.Contains("%"))
                {
                    continue;
                }
                var left = Double.Parse(row.SelectNodes("td")[0].InnerText);
                var type = row.SelectNodes("td")[1].InnerText;
                var right = Double.Parse(row.SelectNodes("td")[2].InnerText);

                switch (type)
                {
                    case "Kicks":
                        match.HomeStats.Kicks = (int)Math.Round(left);
                        match.AwayStats.Kicks = (int)Math.Round(right);
                        break;
                    case "Handballs":
                        match.HomeStats.Handballs = (int)Math.Round(left);
                        match.AwayStats.Handballs = (int)Math.Round(right);
                        break;
                    case "Marks":
                        match.HomeStats.Marks = (int)Math.Round(left);
                        match.AwayStats.Marks = (int)Math.Round(right);
                        break;
                    case "Tackles":
                        match.HomeStats.Tackles = (int)Math.Round(left);
                        match.AwayStats.Tackles = (int)Math.Round(right);
                        break;
                    case "Hitouts":
                        match.HomeStats.HitOuts = (int)Math.Round(left);
                        match.AwayStats.HitOuts = (int)Math.Round(right);
                        break;
                    case "Frees For":
                        match.HomeStats.FreesFor = (int)Math.Round(left);
                        match.AwayStats.FreesFor = (int)Math.Round(right);
                        break;
                    case "Frees Against":
                        match.HomeStats.FreesAgainst = (int)Math.Round(left);
                        match.AwayStats.FreesAgainst = (int)Math.Round(right);
                        break;
                    case "Rushed Behinds":
                        match.HomeStats.RushedBehinds = (int)Math.Round(left);
                        match.AwayStats.RushedBehinds = (int)Math.Round(right);
                        break;
                    case "Clearances":
                        match.HomeStats.Clearances = (int)Math.Round(left);
                        match.AwayStats.Clearances = (int)Math.Round(right);
                        break;
                    case "Clangers":
                        match.HomeStats.Clangers = (int)Math.Round(left);
                        match.AwayStats.Clangers = (int)Math.Round(right);
                        break;
                    case "Rebound 50s":
                        match.HomeStats.Rebound50s = (int)Math.Round(left);
                        match.AwayStats.Rebound50s = (int)Math.Round(right);
                        break;
                    case "Inside 50s":
                        match.HomeStats.Inside50s = (int)Math.Round(left);
                        match.AwayStats.Inside50s = (int)Math.Round(right);
                        break;
                    default:
                        break;
                }
            }
        }

        public void AppendMatchStatisticstoResults(Round round)
        {
            var year = round.Year;
            var roundNo = round.Number;
            var isFinal = numHomeandAwayRounds[year] < roundNo;
            var rounds = numHomeandAwayRounds[year];
            var link = Website + "/afl/footy/ft_match_list?year=" + year;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var web = new HtmlWeb();
            var doc = web.Load(link);

            var nodes = doc.DocumentNode.SelectNodes("//tr[td/a[@name='round_" + roundNo + "']]/following::tr");

            var matchNodes = nodes.Skip(1).TakeWhile(n => n.InnerText != "" && !n.InnerText.Contains("BYE"));

            foreach (var node in matchNodes)
            {
                //Teams
                var home = node.SelectSingleNode("td[2]").SelectSingleNode("a[1]").InnerText;
                var away = node.SelectSingleNode("td[2]").SelectSingleNode("a[2]").InnerText;
                var homeTeam = Team.LoadByName(home);
                var awayTeam = Team.LoadByName(away);
                /*
                var tempTeam = round.Matches[7].Away;
                var tempPlayerMatches = round.Matches[7].AwayPlayerMatches;
                var tempStats = round.Matches[7].AwayStats;


                round.Matches[7].Away = round.Matches[7].Home;
                round.Matches[7].AwayPlayerMatches = round.Matches[7].HomePlayerMatches;
                round.Matches[7].AwayStats = round.Matches[7].HomeStats;

                round.Matches[7].Home = tempTeam;
                round.Matches[7].HomePlayerMatches = tempPlayerMatches;
                round.Matches[7].HomeStats = tempStats;
                */

                var match = round.Matches.First(m => m.Home.Names.Contains(homeTeam.Region) && m.Away.Names.Contains(awayTeam.Region));

                var midTag = node.SelectSingleNode("td[5]").SelectSingleNode("a");

                if (midTag != null)
                {
                    var mid = midTag.GetAttributes("href").First().Value.Replace("ft_match_statistics?mid=", "");
                    AppendMatchStatistics(match, mid);
                }
            }
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
