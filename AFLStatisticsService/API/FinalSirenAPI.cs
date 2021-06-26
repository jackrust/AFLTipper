using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using AustralianRulesFootball;
using HtmlAgilityPack;
using ScreenScraper;
using Match = AustralianRulesFootball.Match;

namespace AFLStatisticsService.API
{
    public class FinalSirenApi : AflStatisticsApi
    {
        private const string Website = "http://finalsiren.com";
        private const string MatchResults = Website + "/Results.asp";
        private const string PlayerList = Website + "/AFLPlayerStats.asp";
        private const string PlayerStats = Website + "/PlayerStats.asp";
        private const int MatchResultTableIndex = 2;
        private const int PlayerTableIndex = 1;


        public override int GetNumRounds(int year)
        {
            if (numHomeandAwayRounds.ContainsKey(year))
            {
                return numHomeandAwayRounds[year];
            }

            var numRounds = 0;
            var parameters = new Dictionary<string, string> { { "SeasonID", year.ToString() }, { "Round", "1-1" } };
            var page = WebsiteAPI.GetPage(MatchResults, parameters);
            var roundList = WebsiteAPI.SplitOn(page, "<select", "</select")[1];

            var r = new Regex("([R,F])([1-9])+");
            var m = r.Match(roundList);
            while (m.Success)
            {
                var round = StringToRoundNumber(m.ToString(), year);
                if (round > numRounds)
                    numRounds = round;
                m = m.NextMatch();
            }
            return numRounds;
        }

        public override Round GetRoundResultsHomeAndAway(int year, int roundNo)
        {
            var isFinal = roundNo > numHomeandAwayRounds[year];
            var parameters = new Dictionary<string, string> { { "SeasonID", year.ToString() }, { "Round", isFinal ? roundNo - numHomeandAwayRounds[year] + "-2" : roundNo + "-1" } };
            var page = WebsiteAPI.GetPage(MatchResults, parameters);

            var link = MatchResults + "?SeasonID=" + year.ToString() + "&Round=" + (isFinal ? roundNo - numHomeandAwayRounds[year] + "-2" : roundNo + "-1");//http://finalsiren.com/Results.asp?SeasonID=2021&Round=12-1

            var web = new HtmlWeb();
            var doc = web.Load(link);

            var nodes = doc.DocumentNode.SelectNodes("//table//tr").Where(n => n.InnerText.Contains("dftd") || n.InnerText.Contains("lost to") || n.InnerText.Contains("drew") || n.InnerHtml.Contains("<td>v</td>")).ToList();

            var matches = new List<Match>();
            var headerCheck = ": Round " + roundNo + " " + year;
            if (!doc.DocumentNode.InnerText.Contains(headerCheck))
                return new Round(Convert.ToInt32(year), roundNo, isFinal, matches);

            foreach (var node in nodes) 
            {
                var details = new List<string>();
                var home = node.SelectNodes("td")[0].FirstChild.InnerText;
                var away = node.SelectNodes("td")[7].FirstChild.InnerText;


                var match = new Match(
                    Util.GetTeamByName(home),
                    new Score(node.SelectNodes("td")[1].InnerText),
                    new Score(node.SelectNodes("td")[2].InnerText) - new Score(node.SelectNodes("td")[1].InnerText),
                    new Score(node.SelectNodes("td")[3].InnerText) - new Score(node.SelectNodes("td")[2].InnerText),
                    new Score(node.SelectNodes("td")[4].InnerText) - new Score(node.SelectNodes("td")[3].InnerText),
                    Util.GetTeamByName(away),
                    new Score(node.SelectNodes("td")[8].InnerText),
                    new Score(node.SelectNodes("td")[9].InnerText) - new Score(node.SelectNodes("td")[8].InnerText),
                    new Score(node.SelectNodes("td")[10].InnerText) - new Score(node.SelectNodes("td")[9].InnerText),
                    new Score(node.SelectNodes("td")[11].InnerText) - new Score(node.SelectNodes("td")[10].InnerText),
                    Util.GetGroundByName(node.SelectNodes("td")[13].SelectSingleNode("a").InnerText),
                    Util.StringToDate(node.SelectNodes("td")[14].InnerText + " " + year.ToString())
                    );
                var statsUrl = Website + node.SelectNodes("td")[15].FirstChild.GetAttributes("href").First().Value.Replace("ft_match_statistics?mid=", "").Replace("&amp;", "&");
                if (statsUrl.Contains("MatchDetails"))
                {
                    var matchStatistics = GetMatchResults(statsUrl, year, roundNo);
                    match.HomeStats = matchStatistics.Item1;
                    match.HomeStats.For = home;
                    match.HomeStats.Against = away;
                    match.AwayStats = matchStatistics.Item2;
                    match.AwayStats.For = away;
                    match.AwayStats.Against = home;
                }
                matches.Add(match);
            }

            return new Round(Convert.ToInt32(year), roundNo, isFinal, matches);
        }

        /*public override Round GetRoundResultsHomeAndAway(int year, int roundNo)
        {
            var isFinal = roundNo > numHomeandAwayRounds[year];
            var parameters = new Dictionary<string, string> { { "SeasonID", year.ToString() }, { "Round", isFinal ? roundNo - numHomeandAwayRounds[year] + "-2" : roundNo + "-1" } };
            var page = WebsiteAPI.GetPage(MatchResults, parameters);
            
            
            var table = WebsiteAPI.SplitOn(page, "<table", "</table")[0];
            var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr", 4);
            var matches = new List<Match>();

            for (var i = MatchResultTableIndex; i < rows.Count; i++)
            {
                var details = WebsiteAPI.SplitOn(rows[i], "<td", "</td", 4);
                var home = ExtractFromXml(details[0]);
                var away = ExtractFromXml(details[7]);

                var match = new Match(
                    Util.GetTeamByName(home),
                    new Score(details[1]),
                    new Score(details[2]) - new Score(details[1]),
                    new Score(details[3]) - new Score(details[2]),
                    new Score(details[4]) - new Score(details[3]),
                    Util.GetTeamByName(away),
                    new Score(details[8]),
                    new Score(details[9]) - new Score(details[8]),
                    new Score(details[10]) - new Score(details[9]),
                    new Score(details[11]) - new Score(details[10]),
                    Util.GetGroundByName(ExtractFromXml(details[13])),
                    Util.StringToDate(details[14] + " " + year.ToString())
                    );

                var gameId = details[15];
                var statisticsParameters = ExtractHrefFromXml(WebsiteAPI.SplitOn(gameId, "<a", "</a", 0)[0]).Replace("&amp;", "&");
                var matchStatistics = GetMatchResults(statisticsParameters, year, roundNo);
                match.HomeStats = matchStatistics.Item1;
                match.HomeStats.For = home;
                match.HomeStats.Against = away;
                match.AwayStats = matchStatistics.Item2;
                match.AwayStats.For = away;
                match.AwayStats.Against = home;
                matches.Add(match);

            }
            return new Round(Convert.ToInt32(year), roundNo, isFinal, matches);
        }*/

        /*public Tuple<MatchStatistics, MatchStatistics> GetMatchResults(string statisticsParameters, int year, int round)
        {
            var home = new MatchStatistics();
            var away = new MatchStatistics();

            home.Year = year;
            home.RoundNo = round;
            away.Year = year;
            away.RoundNo = round;

            if (statisticsParameters.Contains("MatchDetails"))
            {
                var page = WebsiteAPI.GetPage(Website + statisticsParameters);
                var table = WebsiteAPI.SplitOn(page, "<table", "</table")[0];
                var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr", 4);

                FillStatistics(rows, home, 0);
                FillStatistics(rows, away, 2);
            }


            return new Tuple<MatchStatistics, MatchStatistics>(home, away);
        }*/

        public Tuple<MatchStatistics, MatchStatistics> GetMatchResults(string link, int year, int round)
        {
            var web = new HtmlWeb();
            var doc = web.Load(link);
            var home = new MatchStatistics();
            var away = new MatchStatistics();

            home.Year = year;
            home.RoundNo = round;
            away.Year = year;
            away.RoundNo = round;
            var nodes = doc.DocumentNode.SelectNodes("//table/tbody/tr");
            FillStatistics(nodes, home, 0);
            FillStatistics(nodes, away, 2);


            return new Tuple<MatchStatistics, MatchStatistics>(home, away);
        }

        private static MatchStatistics FillStatistics(HtmlNodeCollection nodes, MatchStatistics team, int index)
        {
            team.Kicks = Int32.Parse(nodes.First(n => n.InnerText.Contains("Kicks")).SelectNodes("td")[index].InnerText);
            team.Handballs = Int32.Parse(nodes.First(n => n.InnerText.Contains("Handballs")).SelectNodes("td")[index].InnerText);
            team.Marks = Int32.Parse(nodes.First(n => n.InnerText.Contains("Marks")).SelectNodes("td")[index].InnerText);
            team.HitOuts = Int32.Parse(nodes.First(n => n.InnerText.Contains("Hit Outs")).SelectNodes("td")[index].InnerText);
            team.Tackles = Int32.Parse(nodes.First(n => n.InnerText.Contains("Tackles")).SelectNodes("td")[index].InnerText);
            team.FreesFor = Int32.Parse(nodes.First(n => n.InnerText.Contains("Frees For")).SelectNodes("td")[index].InnerText);
            team.FreesAgainst = Int32.Parse(nodes.First(n => n.InnerText.Contains("Frees Against")).SelectNodes("td")[index].InnerText);
            return team;
        }

        private static string ExtractFromXml(string str)
        {
            return ExtractBetween(str, ">", "<");
        }

        private static string ExtractHrefFromXml(string str)
        {
            return ExtractBetween(str, "href=\"/", "\"");
        }

        private static string ExtractBetween(string str, string first, string last)
        {
            var sFlag = str.IndexOf(first, StringComparison.Ordinal);
            var eFlag = str.IndexOf(last, sFlag + first.Length, StringComparison.Ordinal);
            if (sFlag > -1 && eFlag > -1)
                return str.Substring(sFlag + first.Length, eFlag - (sFlag + first.Length));
            return "";
        }

        //TODO: this function
        public int GetNumPlayerPages(int year)
        {
            var parameters = new Dictionary<string, string>
            {
                {"SeasonID", year.ToString()},
                {"Sort", "Rating%20Desc"}
            };
            var page = WebsiteAPI.GetPage(PlayerList, parameters);
            var navigationPanel = WebsiteAPI.SplitOn(page, "<ul class=\"pagination pagination-sm\"", "</ul")[0];
            var navigaationElements = WebsiteAPI.SplitOn(navigationPanel, "<li", "</li");
            //Ignore the first and last:
            return navigaationElements.Count - 1;
        }

        public override List<Player> GetAllPlayers(int year)
        {
            List<Player> players = new List<Player>();
            var numPages = GetNumPlayerPages(year);

            //Get players for this year
            for (var i = 0; i < numPages; i++)
            {
                players.AddRange(GetPlayers(year, i + 1));
            }
            return players;
        }

        public List<Player> GetPlayers(int year)
        {
            List<Player> players = new List<Player>();
            var numPages = GetNumPlayerPages(year);

            //Get players for this year
            for (var i = 0; i < numPages; i++)
            {
                players.AddRange(GetPlayers(year, i + 1));
            }
            return players;
        }

        //TODO: this function
        public List<Player> GetPlayers(int year, int pageNo)
        {
            var parameters = new Dictionary<string, string>
            {
                {"SeasonID", year.ToString()},
                {"Sort", "Rating%20Desc"},
                {"Page", pageNo.ToString()}
            };
            var page = WebsiteAPI.GetPage(PlayerList, parameters);
            var table = WebsiteAPI.SplitOn(page, "<table", "</table")[0];
            var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr", 4);
            var players = new List<Player>();

            for (var i = PlayerTableIndex; i < (rows.Count - 1); i++)
            {
                var details = WebsiteAPI.SplitOn(rows[i], "<td", "</td", 4);
                var player = new Player()
                {
                    FinalSirenPlayerId = ExtractFinalSirenPlayerId(details[2]),
                    Name = ExtractInnerHtml(details[2]),
                    History = new List<PlayerMatch>()
                };
                players.Add(player);
            }
            return players;
        }

        private int ExtractFinalSirenPlayerId(string tag)
        {
            const string before = "/PlayerStats.asp?PlayerID=";
            const string after = "\"";
            var start = tag.IndexOf(before, StringComparison.Ordinal) + before.Length;
            var end = tag.IndexOf(after, start, StringComparison.Ordinal);

            return ToInt(tag.Substring(start, end - start));
        }

        private string ExtractInnerHtml(string tag)
        {
            const string before = ">";
            const string after = "<";
            var start = tag.IndexOf(before, StringComparison.Ordinal) + before.Length;
            var end = tag.IndexOf(after, start, StringComparison.Ordinal);

            return tag.Substring(start, end - start);
        }

        //TODO: this function
        public List<PlayerMatch> GetPlayerMatchHistory(int playerId)
        {
            //http://finalsiren.com/PlayerStats.asp?PlayerID=1815&SeasonID=ALL#ind

            var parameters = new Dictionary<string, string> { { "PlayerID", playerId.ToString() }, { "SeasonID", "ALL#ind" } };
            var page = WebsiteAPI.GetPage(PlayerStats, parameters);
            var table = WebsiteAPI.SplitOn(page, "<table", "</table")[2];
            var rows = WebsiteAPI.SplitOn(table, "<tr", "</tr");
            var playerMatches = new List<PlayerMatch>();

            for (var i = PlayerTableIndex; i < (rows.Count - 1); i++)
            {
                var details = WebsiteAPI.SplitOn(rows[i], "<td", "</td", 4);
                var playerMatch = new PlayerMatch
                {
                    FinalSirenPlayerId = playerId,
                    Year = ToInt(details[0]),
                    RoundNo = StringToRoundNumber(details[1], ToInt(details[0])),
                    Against = ExtractInnerHtml(details[2]),
                    //Stats
                    Kicks = ToInt(details[3]),
                    Handballs = ToInt(details[4]),
                    Marks = ToInt(details[6]),
                    HitOuts = ToInt(details[7]),
                    Tackles = ToInt(details[8]),
                    FreesFor = ToInt(details[9]),
                    FreesAgainst = ToInt(details[10]),
                    Goals = ToInt(details[11]),
                    Behinds = ToInt(details[12]),
                    Rating = ToInt(details[14]),
                    Win = details[15] == "Win"
                };
                playerMatches.Add(playerMatch);
            }
            return playerMatches;
        }

        private int ToInt(string str)
        {
            return string.IsNullOrWhiteSpace(str) ? 0 : Int32.Parse(str);
        }

        private int StringToRoundNumber(string roundString, int year)
        {

            if (roundString.ToUpper().IndexOf("R", StringComparison.Ordinal) >= 0)
            {
                return ToInt(roundString.Substring(1));
            }
            var number = 0;
            switch (roundString.ToUpper())
            {
                case ("F1"):
                case ("W1"):
                case ("EF"):
                    number = numHomeandAwayRounds[year] + 1;
                    break;
                case ("F2"):
                case ("SF"):
                    number = numHomeandAwayRounds[year] + 2;
                    break;
                case ("F3"):
                case ("PF"):
                    number = numHomeandAwayRounds[year] + 3;
                    break;
                case ("F4"):
                case ("GF"):
                    number = numHomeandAwayRounds[year] + 4;
                    break;
                case ("F5"):
                    number = numHomeandAwayRounds[year] + 5;
                    break;
            }
            return number;
        }

        public override List<Round> GetRoundResultsFinals(int year)
        {
            //throw new NotImplementedException();
            return new List<Round>();
        }
    }
}