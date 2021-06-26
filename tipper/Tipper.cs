using System;
using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class Tipper
    {
        public static int NumInputs = 18;
        public static int NumOutputs = 4;
        public static int DefaultHiddens = 5;
        public League League;
        public List<Cricket.BBLSeason> BBLSeasons;
        public List<Cricket.BBLSeason> WBBLSeasons;
        public Network Net;

        public Tipper()
        {
            var db = new MongoDb();
            League = new League();
            //TODO:REMOVE
            League.Seasons = db.GetSeasons().Where(x => x.Year >= 2003).OrderBy(x => x.Year).ToList();
            BBLSeasons = db.GetBBLSeasons().ToList();
            WBBLSeasons = db.GetWBBLSeasons().ToList();
            /* var playerStats = db.ReadPlayerDocument().ToList();
             foreach (var season in League.Seasons)
             {
                 foreach (var round in season.Rounds)
                 {
                     foreach (var match in round.Matches)
                     {
                         match.HomePlayerMatches = playerStats.SelectMany(x => x.History).Where(
                                         h =>
                                            h.Year == season.Year && h.RoundNo == round.Number &&
                                             match.Away.Equals(Util.GetTeamByName(h.Against))).ToList();
                     }
                 }
             }*/
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
        }

        private void Refresh(int inputs, List<int> hiddens, int outputs)
        {
            Net = new Network(inputs, hiddens, outputs);
        }

        public Data GetMatchDataFromLeagueBetween(RoundShell fromRoundShell, DateTime date)
        {
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
            var round = GetRoundFromDate(date);
            return GetMatchDataFromLeagueBetween(fromRoundShell, round);
        }

        private Round GetRoundFromDate(DateTime date)
        {
            var rounds = League.Seasons.SelectMany(s => s.Rounds).OrderBy(r => r.Matches.OrderBy(m => m.Date).First().Date).ToList();

            if (rounds.SelectMany(r => r.Matches).Max(m => m.Date) < date)
                return rounds.OrderByDescending(r => r.Year).ThenByDescending(r => r.Number).First();
            
            var round = rounds.First(r => r.Matches.OrderBy(m => m.Date).First().Date > date);
            return round;
        }

        public Data GetMatchDataFromLeagueBetween(RoundShell fromRoundShell, RoundShell toRoundShell, DataInterpretation interpretation = null)
        {
            return GetMatchDataBetween(League.Seasons, fromRoundShell, toRoundShell, interpretation);
        }
        
        public static Data GetMatchDataBetween(List<Season> seasons, RoundShell fromRoundShell, RoundShell toRoundShell, DataInterpretation interpretation = null)
        {
            var data = new Data();
            //TODO: It's a little messy to new this up here
            var league = new League(seasons);
            var rounds = league.GetRounds(0, 0, toRoundShell.Year, toRoundShell.Number).Where(x => x.Matches.Count > 0).ToList();
            var matches = rounds.Where(r => (r.EffectiveId() > fromRoundShell.EffectiveId()))
                .SelectMany(r => r.Matches);
            foreach (var m in matches)
            {
                var dataPoint = new DataPoint();
                var history =
                    rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).SelectMany(r => r.Matches).ToList();
                dataPoint.Inputs = (BuildInputs(history, m, interpretation));
                dataPoint.Outputs = (new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Total(), Util.MaxScore),
                    Numbery.Normalise(m.AwayScore().Total(), Util.MaxScore)
                });
                dataPoint.Reference = m.ToTuple();
                data.DataPoints.Add(dataPoint);
            }
            return data;
        }

        public List<Match> PredictNext(DateTime date, bool print)
        {
            var round = GetRoundFromDate(date);
            return Predict(date.Year, round.Number + 1, print);
        }

        public List<Match> Predict(int year, int round, bool print)
        {

            Func<double, double> rule = (m => m > 27.0 ? 15.00 : 0.00);

            var results = new List<Match>();
            var rounds = League.GetRounds(0, 0, year, round).Where(x => x.Matches.Count > 0).ToList();

            foreach (var m in rounds.Where(r => (r.Year == year && r.Number == round)).SelectMany(r => r.Matches))
            {
                var history =
                    rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).SelectMany(r => r.Matches).ToList();
                var test = BuildInputs(history, m);

                var result = Net.Run(test);
                results.Add(new Match(
                    m.Home,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[0], Util.MaxGoals),//, Numbery.NormalisationMethod.Asymptotic),
                        Numbery.Denormalise(result[1], Util.MaxPoints)//, Numbery.NormalisationMethod.Asymptotic)
                        ),
                    m.Away,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[2], Util.MaxGoals),//, Numbery.NormalisationMethod.Asymptotic),
                        Numbery.Denormalise(result[3], Util.MaxPoints)//, Numbery.NormalisationMethod.Asymptotic)
                        ),
                    m.Ground, m.Date));

                var margin = Math.Abs(results.Last().HomeScore().Total() - results.Last().AwayScore().Total());
                var wager = rule(margin);

                if (print)
                    Console.WriteLine("{0,9} Vs {1, 9}: {2}, Bet: ${3:0.00}",
                        m.Home.Mascot, m.Away.Mascot,
                                      Printlayer(new[]
                                      {
                                          results.Last().HomeScore().Goals,
                                          results.Last().HomeScore().Points,
                                          results.Last().HomeScore().Total(),
                                          results.Last().AwayScore().Goals,
                                          results.Last().AwayScore().Points,
                                          results.Last().AwayScore().Total()
                                      }),wager);
            }
            return results;
        }

        public List<PredictedMatch> PredictWinners(int year, int round, bool isFinal, DataInterpretation interpretation = null)
        {
            var predictions = new List<PredictedMatch>();
            var rounds = League.GetRounds(0, 0, year, round).Where(x => x.Matches.Count > 0).ToList();

            foreach (var m in rounds.Where(r => r.Year == year && r.Number == round && r.IsFinal == isFinal).SelectMany(r => r.Matches))
            {
                //TODO: Are we ordering in the right direction?
                var history =
                    rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).SelectMany(r => r.Matches).OrderBy(h => h.Date).ToList();

                var test = BuildInputs(history, m, interpretation);

                var result = Net.Run(test);

                predictions.Add(new PredictedMatch(m.Home, m.Away, m.Ground, m.Date, Numbery.Denormalise(result[0], Util.MaxScore), Numbery.Denormalise(result[1], Util.MaxScore), round));
            }
            return predictions;
        }

        public string ResultToString(List<PredictedMatch> matches)
        {
            var strings = new List<string>();
            foreach (var m in matches)
            {
                strings.Add(String.Format("{0,-5}|{1, -6}|{2, -5}|{3, -6}",
                    Util.GetTeams().Where(t => t.ApiName == m.Home.ApiName).FirstOrDefault().Abbreviation, Printlayer(new[]
                                      {
                                          m.HomeTotal,//, Numbery.NormalisationMethod.Asymptotic),
                                      }), Util.GetTeams().Where(t => t.ApiName == m.Away.ApiName).FirstOrDefault().Abbreviation,
                                      Printlayer(new[]
                                      {
                                          m.AwayTotal//, Numbery.NormalisationMethod.Asymptotic),
                                      })));
            }
            return string.Join("\n", strings.ToArray());
        }

        public string ResultToStringAlt(List<PredictedMatch> matches)
        {
            var strings = new List<string>();
            foreach (var m in matches)
            {
                strings.Add(String.Format("{0,-5} {1, -6}|{2, -5} {3, -6}",
                    Util.GetTeams().Where(t => t.ApiName == m.Home.ApiName).FirstOrDefault().Abbreviation, Printlayer(new[]
                    {
                        Math.Round(m.HomeTotal),//, Numbery.NormalisationMethod.Asymptotic),
                    }).TrimEnd('0').TrimEnd('.'), Util.GetTeams().Where(t => t.ApiName == m.Away.ApiName).FirstOrDefault().Abbreviation,
                    Printlayer(new[]
                    {
                        Math.Round(m.AwayTotal)//, Numbery.NormalisationMethod.Asymptotic),
                    }).TrimEnd('0').TrimEnd('.')));
            }
            return string.Join("\n", strings.ToArray());
        }

        public string ResultToStringTweet(List<PredictedMatch> matches)
        {
            var strings = new List<string>();
            foreach (var m in matches)
            {
                strings.Add(String.Format("{0,-4} {1, -3} - {2, -4} {3, -3}",
                    Util.GetTeams().Where(t => t.ApiName == m.Home.ApiName).FirstOrDefault().Abbreviation, Printlayer(new[]
                    {
                        Math.Round(m.HomeTotal),
                    }).TrimEnd('0').TrimEnd('.'), Util.GetTeams().Where(t => t.ApiName == m.Away.ApiName).FirstOrDefault().Abbreviation,
                    Printlayer(new[]
                    {
                        Math.Round(m.AwayTotal)
                    }).TrimEnd('0').TrimEnd('.')));
            }
            return string.Join("\n", strings.ToArray());
        }

        public void StateMatchesult(int year, int round)
        {
            var rounds = League.GetRounds(0, 0, year, round).Where(x => x.Matches.Count > 0).ToList();

            foreach (var m in rounds.Where(r => (r.Year == year && r.Number == round)).SelectMany(r => r.Matches))
            {
                Console.WriteLine("{0,9}|{1, 9}|{2}",
                    m.Home.Mascot, m.Away.Mascot,
                    Printlayer(new[]
                    {
                        m.HomeScore().Total(),
                        m.AwayScore().Total()
                    }));
            }
        }

        public Data BuildFullDataSet()
        {
            var data = new Data();
            const int fromYear = 2008;
            const int fromRound = 0;
            const int toYear = 2015;
            const int toRound = 24;
            var rounds = League.GetRounds(0, 0, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();
            foreach (
                var m in
                    rounds.Where(r => (r.Year == fromYear && r.Number >= fromRound) || (r.Year > fromYear))
                        .SelectMany(r => r.Matches))
            {

                var history =
                    rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).SelectMany(r => r.Matches).ToList();
                data.DataPoints.Add(AFLDataInterpreterTotal.New().BuildDataPoint(history, m));
            }
            return data;
        }

        public static List<double> BuildInputs(List<Match> history, Match m, DataInterpretation interpretation = null)
        {
            if (interpretation == null)
                interpretation = AFLDataInterpreter.BespokeLegacyInterpretation;

            var input = AFLDataInterpreterTotal.New().BuildInputs(history, m, interpretation);
            return input;
        }

        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("", (current, t) => current + String.Format("{0:N1}|", t));
            return result.TrimEnd(' ').TrimEnd('|') + "";
        }

        //BBL
        #region BBL
        public Data GetMatchDataBetween(int fromID, int toID, List<List<int>> interpretation = null)
        {
            var matches = BBLSeasons.SelectMany(x => x.Matches).ToList();
            var data = new Data();
            foreach (var m in matches.Where(m => m.EffectiveID() >= fromID && m.EffectiveID() <= toID))
            {
                var dataPoint = new DataPoint();
                var history =
                    matches.Where(x => x.EffectiveID() < m.EffectiveID()).ToList();
                dataPoint.Inputs = (BuildInputs(history, m, interpretation));
                dataPoint.Outputs = (new List<double>()
                {
                    //m.Tie() ? 0.5 : (m.HomeWin() ? 1 : 0),
                    //m.Tie() ? 0.5 : (m.AwayWin() ? 1 : 0)
                    m.HomeWinningness(),
                    m.AwayWinningness(),
                });
                dataPoint.Reference = m.EffectiveID();
                //Console.WriteLine("({0:p2}, {1:p2}) {2} {3}", dataPoint.Outputs[0], dataPoint.Outputs[1], m.HomeWin(), m.AwayWin());
                data.DataPoints.Add(dataPoint);
            }
            return data;
        }

        public static List<double> BuildInputs(List<Cricket.Match> history, Cricket.Match m, List<List<int>> interpretation = null)
        {
            if (interpretation == null)
                interpretation = BBLDataInterpreter.Interpretations.Default;

            var input = BBLDataInterpreterWin.New().BuildInputs(history, m, interpretation);
            return input;
        }

        public List<Cricket.PredictedMatch> PredictBBLWinners(int year, int number, List<List<int>> interpretation = null)
        {
            var predictions = new List<Cricket.PredictedMatch>();
            var matches = BBLSeasons.SelectMany(s => s.Matches).Where(m => (m.Date.Year == year && m.Number == number));

            foreach (var m in matches)
            {
                var history =
                    BBLSeasons.SelectMany(s => s.Matches).Where(x => x.EffectiveID() < m.EffectiveID()).OrderBy(h => h.EffectiveID()).ToList();

                var test = BuildInputs(history, m, interpretation);

                var result = Net.Run(test);

                predictions.Add(new Cricket.PredictedMatch(m.Home, m.Away, m.Ground, m.Date, number, result[0], result[1]));
            }
            return predictions;
        }
        #endregion
    }
}
