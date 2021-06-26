using AustralianRulesFootball;
using Cricket;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AFLStatisticsService.API
{
    class MyKhelAPI
    {
        private int FirstYear = 2011;

        internal List<BBLSeason> UpdateFrom(int year, List<BBLSeason> seasons)
        {
            year = year < FirstYear ? FirstYear : year;
            if (seasons.Count <= 1)
                seasons = new List<BBLSeason>();

            var SeasonUrls = GetSeasonUrls();

            for (var i = year; i <= DateTime.Now.Year; i++)
            {
                var url = SeasonUrls.Where(u => u.Contains(i + "-" + (i -2000 + 1))).FirstOrDefault();
                var season = new BBLSeason();
                if (url != null)
                {
                    season = GetSeason(url);

                    seasons.RemoveAll(s => s.Year == i);

                    seasons.Add(season);

                }
            }

            return seasons;
        }

        private List<string> GetSeasonUrls()
        {
            var link = @"https://www.mykhel.com/cricket/big-bash-league-2011-12-schedule-results-s9954/";
            var web = new HtmlWeb();
            var doc = web.Load(link);
            var nodes = doc.DocumentNode.SelectNodes("//select[@id='non-opta-season']/option");
            return nodes.Select(n => n.Attributes["value"].Value).ToList();
        }

        private BBLSeason GetSeason(string url)
        {
            var season = new BBLSeason();
            var link = "https://www.mykhel.com/" + url;
            var web = new HtmlWeb();
            var doc = web.Load(link);
            var nodes = doc.DocumentNode.SelectNodes("//tr[contains(@class,'result')]");

            foreach(var node in nodes)
            {
                var match = new Cricket.Match();
                var segments = node.SelectSingleNode("td").SelectSingleNode("table").SelectSingleNode("tbody").SelectSingleNode("tr").SelectNodes("td");
                var dateText = segments[0].SelectNodes("div")[0].InnerText;
                var groundText = segments[0].SelectNodes("div")[1].InnerText;
                var roundText = segments[0].InnerText.Replace(dateText, "").Replace(groundText, "");

                var number = 0;
                switch (roundText)
                {
                    case ("Final 1,  "):
                        number = 3;
                        break;
                }
                match.Number = number;
                match.Date = Util.StringToDate(dateText);
                match.Ground = Util.GetGroundByName(groundText.Trim().Split(',')[0]);


                var home = segments[1].SelectNodes("div")[0].SelectNodes("span")[0].InnerText.Trim();
                var away = segments[1].SelectNodes("div")[1].SelectNodes("span")[0].InnerText.Trim();
                match.Home = Cricket.Team.FindByName(home);
                match.Away = Cricket.Team.FindByName(away);

                var resultText = segments[2].InnerText;

                if (segments[2].InnerHtml.Contains("No Result"))
                {
                    match.HomeScore = new MatchScore();
                    match.AwayScore = new MatchScore();
                    match.Abandoned = true;
                    match.Result.Victor = Victor.Abandoned;
                }
                else
                {
                    match.HomeScore = MatchScore.Str2ScoreEnglish(segments[1].SelectNodes("div")[0].SelectNodes("span")[1].InnerText.Trim());
                    match.AwayScore = MatchScore.Str2ScoreEnglish(segments[1].SelectNodes("div")[1].SelectNodes("span")[1].InnerText.Trim());



                    match.Result.DuckworthLewisStern = resultText.Contains("(D/L)") || resultText.Contains("(DLS)");
                    if (resultText.Contains(home))
                        match.Result.Victor = Victor.Home;
                    else if (resultText.Contains(away))
                        match.Result.Victor = Victor.Away;
                    else
                        match.Result.Victor = Victor.Draw;

                    var margin = resultText.Replace(match.Home.Names[2], "").Replace(match.Away.Names[2], "").Replace("won by", "").Replace("runs", "").Replace("wickets", "").Replace("(D/L)", "").Replace("(DLS)", "").Trim();

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
