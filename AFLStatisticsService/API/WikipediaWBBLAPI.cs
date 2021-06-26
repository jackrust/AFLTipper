using AustralianRulesFootball;
using Cricket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AFLStatisticsService.API
{
    class WikipediaWBBLAPI
    {
        private int FirstYear = 2015;

        internal List<BBLSeason> UpdateFrom(int year, List<BBLSeason> seasons)
        {
            year = year < FirstYear ? FirstYear : year;
            if (seasons.Count <= 1)
                seasons = new List<BBLSeason>();

            for (var i = year; i <= DateTime.Now.Year; i++)
            {
                var url = "https://en.wikipedia.org/wiki/" + year + "–" + (year + 1).ToString().Replace("20", "") + "_Women%27s_Big_Bash_League_season";
                var season = new BBLSeason();
                if (url != null)
                {
                    season = GetSeason(url, i);

                    seasons.RemoveAll(s => s.Year == i);

                    seasons.Add(season);

                }
            }

            return seasons;
        }

        private BBLSeason GetSeason(string url, int year)
        {
            var season = new BBLSeason();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            var web = new HtmlWeb();
            var doc = web.Load(url);
            var nodes = doc.DocumentNode.SelectNodes("//div[contains(@style,'width: 100%; clear:both')]");
            var newYear = false;
            var dateYear = year;

            foreach(var node in nodes)
            {
                var match = new Cricket.Match();
                var segments = node.SelectNodes("table");//.SelectSingleNode("tbody").SelectSingleNode("tr").SelectSingleNode("td").SelectSingleNode("div");
                var dateText = segments[0].SelectSingleNode("tbody").SelectSingleNode("tr").SelectSingleNode("td").SelectSingleNode("div").GetDirectInnerText().Replace("()", "").Trim();
                if(dateText.Contains("January") && !newYear)
                {
                    newYear = true;
                    dateYear++;
                }
                dateText += " " + dateYear;
                var resultText = segments[2].SelectSingleNode("tbody").SelectSingleNode("tr").SelectSingleNode("td").SelectSingleNode("div").SelectSingleNode("b").GetDirectInnerText();
                var groundText = segments[2].SelectSingleNode("tbody").SelectSingleNode("tr").SelectSingleNode("td").SelectSingleNode("div").SelectSingleNode("small").SelectNodes("a")[0].InnerText;

                match.Date = Util.StringToDate(dateText);
                match.Ground = Util.GetGroundByName(groundText.Trim().Split(',')[0]);

                var home = segments[1].SelectSingleNode("tbody").SelectSingleNode("tr").SelectNodes("td")[0].SelectSingleNode("div").SelectSingleNode("b").SelectSingleNode("a").InnerText;
                var away = segments[1].SelectSingleNode("tbody").SelectSingleNode("tr").SelectNodes("td")[2].SelectSingleNode("div").SelectSingleNode("b").SelectSingleNode("a").InnerText;
                match.Home = Cricket.Team.FindByName(home);
                match.Away = Cricket.Team.FindByName(away);

                

                if (resultText.Contains("No Result"))
                {
                    match.HomeScore = new MatchScore();
                    match.AwayScore = new MatchScore();
                    match.Abandoned = true;
                    match.Result.Victor = Victor.Abandoned;
                }
                else
                {
                    match.HomeScore = MatchScore.Str2ScoreAustralian(segments[1].SelectSingleNode("tbody").SelectSingleNode("tr").SelectNodes("td")[0].SelectSingleNode("div").GetDirectInnerText().Trim());
                    match.AwayScore = MatchScore.Str2ScoreAustralian(segments[1].SelectSingleNode("tbody").SelectSingleNode("tr").SelectNodes("td")[2].SelectSingleNode("div").GetDirectInnerText().Trim());



                    match.Result.DuckworthLewisStern = resultText.Contains("(D/L)") || resultText.Contains("(DLS)");
                    if (resultText.Contains(home))
                        match.Result.Victor = Victor.Home;
                    else if (resultText.Contains(away))
                        match.Result.Victor = Victor.Away;
                    else
                        match.Result.Victor = Victor.Draw;

                    var margin = resultText.Replace(match.Home.Names[2], "").Replace(match.Away.Names[2], "").Replace("won by", "").Replace("runs", "").Replace("wickets", "").Replace("(D/L)", "").Replace("(DLS)", "").Trim();
                    margin = margin.Split('(')[0];
                    if (resultText.Contains("runs"))
                        match.Result.MarginByRuns = Int32.Parse(margin);
                    if (resultText.Contains("wickets"))
                        match.Result.MarginByWickets = Int32.Parse(margin);
                }

                season.Matches.Add(match);
            }

            var i = 0;
            foreach (var m in season.Matches.OrderBy(m => m.Date))
            {
                i++;
                m.Number = i;
            }
            season.Year = season.Matches.OrderBy(m => m.Date).First().Date.Year;
            return season;
        }
    }
}
