using System;
using System.Collections.Generic;
using System.Linq;
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
            League = League.Load();
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
            var seasons = League.Seasons;
            var rounds = new List<Round>();
            var matches = new List<Match>();
            foreach (var s in seasons)
            {
                rounds.AddRange(s.Rounds);
            }
            foreach (var r in rounds)
            {
                matches.AddRange(r.Matches);
            }
            var match = matches.OrderByDescending(m => m.Date).First(m => m.Date < date);
            var round = rounds.First(r => r.Matches.Any(m => m.Equals(match)));
            return round;
        }

        public Data GetMatchDataBetween(int fromYear, int fromRound, int toYear, int toRound)
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
                datapoint.Inputs = (BuildInputs(history, m));
                datapoint.Outputs = (new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.HomeScore().Points, Util.MaxPoints),
                    Numbery.Normalise(m.AwayScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.AwayScore().Points, Util.MaxPoints),
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
                        Numbery.Denormalise(result[0], Util.MaxGoals),
                        Numbery.Denormalise(result[1], Util.MaxPoints)
                        ),
                    m.Away,
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(0, 0),
                    new Score(
                        Numbery.Denormalise(result[2], Util.MaxGoals),
                        Numbery.Denormalise(result[3], Util.MaxPoints)
                        ),
                    m.Ground, m.Date));

                if (print)
                    Console.WriteLine(m.Home.Mascot + " Vs " + m.Away.Mascot + ": " +
                                      Printlayer(new[]
                                      {
                                          results.Last().HomeScore().Goals,
                                          results.Last().HomeScore().Points,
                                          results.Last().HomeScore().Total(),
                                          results.Last().AwayScore().Goals,
                                          results.Last().AwayScore().Points,
                                          results.Last().AwayScore().Total()
                                      }));
            }
            return results;
        }

        public Data BuildFullDataSet()
        {
            var data = new Data();
            const int fromYear = 0;
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
                data.DataPoints.Add(AFLDataInterpreter.BuildDataPoint(history, m));
            }
            return data;
        }

        public static List<double> BuildInputs(List<Match> history, Match m)
        {
            var input = AFLDataInterpreter.BuildInputs(history, m,
                AFLDataInterpreter.Interpretations.BespokeLegacyInterpretation);
            return input;
        }

        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("{", (current, t) => current + String.Format("{0:N1}, ", t));
            return result + "}";
        }
    }
}
