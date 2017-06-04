using System;
using System.Collections.Generic;
using System.EnterpriseServices;
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
        public Network Net;

        public Tipper()
        {
            var db = new MongoDb();
            League = new League();
            //TODO:REMOVE
            League.Seasons = db.ReadSeasonDocument().Where(x => x.Year >= 2003).ToList();
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
        }

        private void Refresh(int inputs, List<int> hiddens, int outputs)
        {
            Net = new Network(inputs, hiddens, outputs);
        }

        public Data GetMatchDataBetween(int fromYear, int fromRound, DateTime date)
        {
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
            var round = GetRoundFromDate(date);
            return GetMatchDataBetween(fromYear, fromRound, date.Year, round.Number);
        }

        private Round GetRoundFromDate(DateTime date)
        {
            var rounds = League.Seasons.SelectMany(s => s.Rounds).OrderBy(r => r.Matches.OrderBy(m => m.Date).First().Date).ToList();

            if (rounds.SelectMany(r => r.Matches).Max(m => m.Date) < date)
                return rounds.OrderByDescending(r => r.Year).ThenByDescending(r => r.Number).First();
            
            var round = rounds.First(r => r.Matches.OrderBy(m => m.Date).First().Date > date);
            return round;
        }

        public Data GetMatchDataBetween(int fromYear, int fromRound, int toYear, int toRound, List<List<int>> interpretation = null)
        {
            var data = new Data();
            var rounds = League.GetRounds(0, 0, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();
            var matches = rounds.Where(r => (r.Year == fromYear && r.Number >= fromRound) || (r.Year > fromYear))
                .SelectMany(r => r.Matches);
            foreach (var m in matches)
            {
                var datapoint = new DataPoint();
                var history =
                    rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).SelectMany(r => r.Matches).ToList();
                datapoint.Inputs = (BuildInputs(history, m, interpretation));
                datapoint.Outputs = (new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Total(), Util.MaxScore),
                    Numbery.Normalise(m.AwayScore().Total(), Util.MaxScore)
                });
                datapoint.Reference = m;
                data.DataPoints.Add(datapoint);
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

        public void PredictWinner(int year, int round, bool print, List<List<int>> interpretation = null)
        {
            var rounds = League.GetRounds(0, 0, year, round).Where(x => x.Matches.Count > 0).ToList();

            foreach (var m in rounds.Where(r => (r.Year == year && r.Number == round)).SelectMany(r => r.Matches))
            {
                var history =
                    rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).SelectMany(r => r.Matches).OrderBy(h => h.Date).ToList();

                var test = BuildInputs(history, m, interpretation);

                var result = Net.Run(test);

                if (print)
                    Console.WriteLine("{0,9}|{1, 9}|{2}",
                        m.Home.Mascot, m.Away.Mascot,
                                      Printlayer(new[]
                                      {
                                          Numbery.Denormalise(result[0], Util.MaxScore),//, Numbery.NormalisationMethod.Asymptotic),
                                          Numbery.Denormalise(result[1], Util.MaxScore)//, Numbery.NormalisationMethod.Asymptotic),
                                      }));
            }
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

        public static List<double> BuildInputs(List<Match> history, Match m, List<List<int>> interpretation = null)
        {
            if (interpretation == null)
                interpretation = AFLDataInterpreter.Interpretations.BespokeLegacyInterpretation;

            var input = AFLDataInterpreterTotal.New().BuildInputs(history, m, interpretation);
            return input;
        }

        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("", (current, t) => current + String.Format("{0:N1}|", t));
            return result.TrimEnd(' ').TrimEnd('|') + "";
        }
    }
}
