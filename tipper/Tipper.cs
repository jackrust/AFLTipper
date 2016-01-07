using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class Tipper
    {
        public static int NumInputs = 18;
        public static int NumOutputs = 4;
        public static int DefaultHiddens = 5;
        public static int RelevantRoundHistory = 14;
        public List<Team> Teams;
        public League League;
        public Network Net;

        public Tipper()
        {
            Teams = Util.GetTeams();
            League = League.Load();
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
        }

        public void Refresh(int maxEpochs, double targetError, int inputs, List<int> hiddens, int outputs)
        {
            Refresh(inputs, hiddens, outputs);
            Net.MaxEpochs = maxEpochs;
            Net.TargetError = targetError;
        }

        public void Refresh(int inputs, List<int> hiddens, int outputs)
        {
            Net = new Network(inputs, hiddens, outputs);
        }

        public Data LearnFromTo(int fromYear, int fromRound, DateTime date)
        {
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
            var round = GetRoundFromDate(date);
            return LearnFromTo(fromYear, fromRound, date.Year, round.Number);
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

        public Data LearnFromScratchFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
            return LearnFromTo(fromYear, fromRound, toYear, toRound);
        }

        public Data LearnFromTo(RoundShell roundFrom, RoundShell roundTo)
        {
            return LearnFromTo(roundFrom.Year, roundFrom.Number, roundTo.Year, roundTo.Number);
        }

        public Data LearnFromTo(int fromYear, int fromRound, int toYear, int toRound, int minMargin = 0, int minTotalScore = 0)
        {
            var data = new Data();
            var rounds = League.GetRounds(0, 0, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();
            foreach (var m in rounds.Where(r => (r.Year == fromYear && r.Number >= fromRound) || (r.Year > fromYear)).SelectMany(r => r.Matches))//.Where(m => m.Margin() >= minMargin && m.TotalScore() >= minTotalScore))
            {
                var datapoint = new DataPoint();
                var season = new Season(toYear, rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).ToList());
                datapoint.Inputs = (BuildInputs(season, m));
                datapoint.Outputs = (new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.HomeScore().Points, Util.MaxPoints),
                    Numbery.Normalise(m.AwayScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.AwayScore().Points, Util.MaxPoints),
                });
                datapoint.Reference = (m.Home.ApiName + " Vs " + m.Away.ApiName);
                data.DataPoints.Add(datapoint);
            }
            return data;
        }

        public List<Match> PredictNext(DateTime date, bool print)
        {
            var round = GetRoundFromDate(date);
            return Predict(date.Year, round.Number+1, print);
        }

        public List<Match> Predict(RoundShell round)
        {
            return Predict(round.Year, round.Number, false);
        }

        public List<Match> Predict(RoundShell round, bool print)
        {
            return Predict(round.Year, round.Number, print);
        }

        public List<Match> Predict(int year, int round, bool print)
        {
            var results = new List<Match>();
            var rounds = League.GetRounds(0, 0, year, round).Where(x => x.Matches.Count > 0).ToList();

            foreach (var m in rounds.Where(r => (r.Year == year && r.Number == round)).SelectMany(r => r.Matches))
            {
                var s = new Season(year, rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).ToList());
                var test = BuildInputs(s, m);

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

        public static double ExtractInput(List<Match> s, Func<Match, bool> wherePredicate, int takeLength, Func<Match, double> sumSelector, Func<double, double> maxFunc)
        {
            var value = s
                .Where(wherePredicate)
                .OrderByDescending(x => x.Date)
                .Take(takeLength)
                .Sum(sumSelector);
            var max = maxFunc(
                s
                    .Where(wherePredicate)
                    .OrderByDescending(x => x.Date)
                    .Take(takeLength)
                    .Count());
            return Numbery.Normalise(value, max);
        }

        public static List<double> BuildInputs(Season s, Match m)
        {
            const int previous = 1;
            const int shortTerm = 3;
            const int midTerm = 6;
            const int longTerm = 10;
            const int longerTerm = 15;

            const int relevantYearsDifference = -12;

            var matches = s.GetMatches();


            var input = new List<double>();
            //------------------------------------------------------------------------------------------------------------------------
            //Scores By Team longerTerm -0.067421444
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores By Team longTerm -0.225011349
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Team midTerm -0.224657876
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores By Team shortTerm -0.007401804
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Team previous -0.058895993
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //------------------------------------------------------------------------------------------------------------------------
            //Scores By Ground longerTerm -0.29332889
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Ground longTerm -0.20326023
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Ground midTerm -0.171834264
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores By Ground shortTerm -0.244169733
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Ground previous -0.045095139
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //------------------------------------------------------------------------------------------------------------------------
            //Scores By State longerTerm
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm * 2, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Ground.State longTerm
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Ground.State midTerm
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), midTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores By Ground.State shortTerm
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores By Ground previous
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), previous, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));


            //------------------------------------------------------------------------------------------------------------------------
            //Scores by Day - longerTerm -0.141145875
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores by Day - longTerm -0.093211292
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores by Day - midTerm -0.799973192
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));
                                                                                                              
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Scores by Day - shortTerm -0.207297888
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));
                                                                                                              
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Scores by Day - 8 -0.017880142
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), previous, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), previous, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), previous, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), previous, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //------------------------------------------------------------------------------------------------------------------------
            //Recent Opponents
            var recentOpponentsHome = matches.Where(mtch => mtch.HasTeam(m.Home)).OrderByDescending(x => x.Date).Take(midTerm).Select(mtch => mtch.GetOpposition(m.Home)).ToList();
            var recentOpponentsAway = matches.Where(mtch => mtch.HasTeam(m.Away)).OrderByDescending(x => x.Date).Take(midTerm).Select(mtch => mtch.GetOpposition(m.Away)).ToList();

            //Recent Opponents -0.342167912
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(recentOpponentsHome).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(recentOpponentsAway).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(recentOpponentsHome).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(recentOpponentsAway).Points), GetMaxSeasonPoints));

            ////Recent Opponents -0.002593704
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(recentOpponentsAway).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(recentOpponentsAway).Points), GetMaxSeasonPoints));

            ////Recent Opponents -0.043971845
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(recentOpponentsAway).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(recentOpponentsAway).Points), GetMaxSeasonPoints));

            ////Recent Opponents -0.282854293
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(recentOpponentsAway).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(recentOpponentsAway).Points), GetMaxSeasonPoints));

            ////Recent Opponents -0.244596983
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), previous, (x => x.ScoreFor(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), previous, (x => x.ScoreFor(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), previous, (x => x.ScoreFor(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), previous, (x => x.ScoreFor(recentOpponentsAway).Points), GetMaxSeasonPoints));
                                                                                                                 
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(recentOpponentsHome).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsHome) && !mtch.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(recentOpponentsHome).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(recentOpponentsAway).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(matches, (mtch => mtch.HasTeam(recentOpponentsAway) && !mtch.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(recentOpponentsAway).Points), GetMaxSeasonPoints));


            //------------------------------------------------------------------------------------------------------------------------
            
            var recentMatchesHome = matches.Where(mtch => mtch.HasTeam(m.Home) && !mtch.HasTeam(m.Away)).OrderByDescending(mtch => mtch.Date).Take(longerTerm).ToList();
            var recentMatchesAway = matches.Where(mtch => mtch.HasTeam(m.Away) && !mtch.HasTeam(m.Home)).OrderByDescending(mtch => mtch.Date).Take(longerTerm).ToList();
            var recentMatchesSharedOpponentHome = recentMatchesHome.Where(hmtch => hmtch.HasTeam(recentMatchesAway.Select(amtch => amtch.GetOpposition(m.Away)).ToList())).ToList();
            var recentMatchesSharedOpponentAway = recentMatchesAway.Where(amtch => amtch.HasTeam(recentMatchesHome.Select(hmtch => hmtch.GetOpposition(m.Home)).ToList())).ToList();

            //Recent Shared Opponents -0.060799118
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longerTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longerTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //Recent Shared Opponents -0.226045649
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Recent Shared Opponents -0.424578264
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), midTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), midTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Recent Shared Opponents -0.002881552
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), shortTerm, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), shortTerm, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            ////Recent Shared Opponents -0.016136036
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints));

            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentHome, (x => x.HasTeam(m.Home)), previous, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals));
            //input.Add(ExtractInput(recentMatchesSharedOpponentAway, (x => x.HasTeam(m.Away)), previous, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints));

            //------------------------------------------------------------------------------------------------------------------------
            //Streak by Team - 2 -0.026442886
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home)), longTerm, (x => x.ScoreFor(m.Home).Total() > x.ScoreAgainst(m.Home).Total() ? 1 : 0), SimpleCount));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away)), longTerm, (x => x.ScoreFor(m.Away).Total() > x.ScoreAgainst(m.Away).Total() ? 1 : 0), SimpleCount));

            //Experience at Ground -0.107465767
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => x.Ground.Equals(m.Ground) ? 1 : 0), SimpleCount));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => x.Ground.Equals(m.Ground) ? 1 : 0), SimpleCount));

            //Experience in state -0.217582948
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => x.Ground.State.Equals(m.Ground.State) ? 1 : 0), SimpleCount));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => x.Ground.State.Equals(m.Ground.State) ? 1 : 0), SimpleCount));

            //Experience in month -0.024616379
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => x.Date.Month == m.Date.Month ? 1 : 0), SimpleCount));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => x.Date.Month == m.Date.Month ? 1 : 0), SimpleCount));

            //Distance from middle of winter -3.26764878
            var winterSolstice = new DateTime(m.Date.Year, 6, 22);
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => Math.Abs((x.Date-winterSolstice).Days)), SimpleCount));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => Math.Abs((x.Date-winterSolstice).Days)), SimpleCount));

            var seasonStart = new DateTime(m.Date.Year, 4, 1);
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => Math.Abs((x.Date - seasonStart).Days)), SimpleCount));
            //input.Add(ExtractInput(matches, (x => x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference)), longerTerm, (x => Math.Abs((x.Date - seasonStart).Days)), SimpleCount));


            //Days between games -0.3836391
            var previousGameHome = matches.Where(x => x.HasTeam(m.Home)).OrderByDescending(x => x.Date).FirstOrDefault();
            var previousGameAway = matches.Where(x => x.HasTeam(m.Away)).OrderByDescending(x => x.Date).FirstOrDefault();
            var datediffHome = previousGameHome == null ? GetMaxDaysBetweenGames() : (m.Date - previousGameHome.Date).Days;
            var datediffAway = previousGameAway == null ? GetMaxDaysBetweenGames() : (m.Date - previousGameAway.Date).Days;
            //input.Add(Numbery.Normalise(datediffHome, GetMaxDaysBetweenGames()));
            //input.Add(Numbery.Normalise(datediffAway, GetMaxDaysBetweenGames()));

            return input;
        }

        public int MarkRound(int round, List<Match> tips)
        {
            var matches = League.GetCurrentSeason().Rounds[round].Matches;
            return tips.Count != matches.Count ? 0 : matches.Sum(t => tips.Count(t1 => t.GetLosingTeam().Equals(t1.GetLosingTeam())));
        }

        public int MarkRound(int year, int round, List<Match> tips)
        {
            var matches = League.GetSeason(year).Rounds[round].Matches;
            return tips.Count != matches.Count ? 0 : matches.Sum(t => tips.Count(t1 => t.GetLosingTeam().Equals(t1.GetLosingTeam())));
        }

        public static double GetMaxLadderPoints(double rounds)
        {
            return rounds * 4;
        }
        public static double GetMaxRLadderPoints(double rounds)
        {
            return rounds * 4 + rounds * rounds * 2;
        }
        public static double GetMaxCLadderPoints(double rounds)
        {
            return Numbery.Factorial(rounds);
        }
        public static double GetMaxSeasonScores(double rounds)
        {
            return Util.MaxScore * rounds;
        }
        public static double GetMaxSeasonGoals(double rounds)
        {
            return Util.MaxGoals * rounds;
        }
        public static double GetMaxSeasonPoints(double rounds)
        {
            return Util.MaxPoints * rounds;
        }

        public static double SimpleCount(double rounds)
        {
            return rounds;
        }

        private static int GetMaxDaysBetweenGames()
        {
            return 21;//TODO: find the actual max in season
        }

        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("{", (current, t) => current + String.Format("{0:N1}, ", t));
            return result + "}";
        }
    }
}
