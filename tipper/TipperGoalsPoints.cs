using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class TipperGoalsPoints : Tipper
    {
        public static int NumInputs = 18;
        public static int NumOutputs = 4;
        public static int DefaultHiddens = 5;
        public static int RelevantRoundHistory = 14;
        public List<Team> Teams;
        public League League;
        public Network Net;

        public TipperGoalsPoints()
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

        public Data LearnFromScratchFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            Refresh(NumInputs, new List<int>() { DefaultHiddens }, NumOutputs);
            return LearnFromTo(fromYear, fromRound, toYear, toRound);
        }

        public Data LearnFromTo(RoundShell roundFrom, RoundShell roundTo)
        {
            return LearnFromTo(roundFrom.Year, roundFrom.Number, roundTo.Year, roundTo.Number);
        }

        public Data LearnFromTo(int fromYear, int fromRound, int toYear, int toRound)
        {
            var references = new List<string>();
            var inputs = new List<List<double>>();
            var targets = new List<List<double>>();
            var rounds = League.GetRounds(0, 0, toYear, toRound).Where(x => x.Matches.Count > 0).ToList();
            foreach (var m in rounds.Where(r => (r.Year == fromYear && r.Number >= fromRound) || (r.Year > fromYear)).SelectMany(r => r.Matches))
            {
                var season = new Season(toYear, rounds.Where(r => !r.Matches.Any(rm => rm.Date >= m.Date)).ToList());
                inputs.Add(BuildInputs(season, m));
                targets.Add(new List<double>()
                {
                    Numbery.Normalise(m.HomeScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.HomeScore().Points, Util.MaxPoints),
                    Numbery.Normalise(m.AwayScore().Goals, Util.MaxGoals),
                    Numbery.Normalise(m.AwayScore().Points, Util.MaxPoints),
                });
                references.Add(m.Home.ApiName + " Vs " + m.Away.ApiName);
            }
            var data = new Data()
            {
                References = references,
                Inputs = inputs,
                Outputs = targets
            };
            return data;
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

        private static List<double> BuildInputs(Season s, Match m)
        {
            const int shortTerm = 4;
            const int midTerm = 8;
            const int longTerm = 12;

            var input = new List<double>();
            //Scores By Team - 8
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreFor(m.Home).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreFor(m.Home).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreFor(m.Away).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Away)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreFor(m.Away).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Away)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreAgainst(m.Home).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreAgainst(m.Home).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreAgainst(m.Away).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Away)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(midTerm)
                        .Sum(x => x.ScoreAgainst(m.Away).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(midTerm)
                            .Count(x => x.HasTeam(m.Away)))));

            //Scores By Team Recent - 8
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreFor(m.Home).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreFor(m.Home).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreFor(m.Away).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Away)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreFor(m.Away).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Away)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreAgainst(m.Home).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreAgainst(m.Home).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Home))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Home)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreAgainst(m.Away).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Away)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.ScoreAgainst(m.Away).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Where(x => x.HasTeam(m.Away))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.HasTeam(m.Away)))));

            //Scores By Ground int the last 2 years- 8
            input.Add(Numbery.Normalise(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.HomeScore().Goals),
                GetMaxSeasonGoals(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.Ground.Equals(m.Ground)))));
            input.Add(Numbery.Normalise(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.HomeScore().Points),
                GetMaxSeasonPoints(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.Ground.Equals(m.Ground)))));
            input.Add(Numbery.Normalise(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.AwayScore().Goals),
                GetMaxSeasonGoals(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.Ground.Equals(m.Ground)))));
            input.Add(Numbery.Normalise(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                        .OrderByDescending(x => x.Date)
                        .Take(shortTerm)
                        .Sum(x => x.AwayScore().Points),
                GetMaxSeasonPoints(s.GetMatches()
                .Where(x => x.Ground.Equals(m.Ground))
                            .OrderByDescending(x => x.Date)
                            .Take(shortTerm)
                            .Count(x => x.Ground.Equals(m.Ground)))));

            //Scores by Day - 8
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek).Sum(x => x.ScoreFor(m.Home).Goals),
                GetMaxSeasonGoals(
                    s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek).Sum(x => x.ScoreFor(m.Home).Points),
                GetMaxSeasonPoints(
                    s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek).Sum(x => x.ScoreFor(m.Away).Goals),
                GetMaxSeasonGoals(
                    s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek).Sum(x => x.ScoreFor(m.Away).Points),
                GetMaxSeasonPoints(
                    s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm)
                    .Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek)
                    .Sum(x => x.ScoreAgainst(m.Home).Goals),
                GetMaxSeasonGoals(
                    s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm)
                    .Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek)
                    .Sum(x => x.ScoreAgainst(m.Home).Points),
                GetMaxSeasonPoints(
                    s.GetMatches().Where(x => x.HasTeam(m.Home))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm)
                    .Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek)
                    .Sum(x => x.ScoreAgainst(m.Away).Goals),
                GetMaxSeasonGoals(
                    s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)))));
            input.Add(Numbery.Normalise(
                s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm)
                    .Where(x => x.Date.DayOfWeek == m.Date.DayOfWeek)
                    .Sum(x => x.ScoreAgainst(m.Away).Points),
                GetMaxSeasonPoints(
                    s.GetMatches().Where(x => x.HasTeam(m.Away))
                        .OrderByDescending(x => x.Date)
                        .Take(longTerm).Count(x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away)))));

            //Head To Head (Last 2 years)
            /*input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreFor(m.Home).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreFor(m.Home).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreFor(m.Away).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreFor(m.Away).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreAgainst(m.Home).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreAgainst(m.Home).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreAgainst(m.Away).Goals),
                    GetMaxSeasonGoals(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));
            input.Add(
                Numbery.Normalise(
                    s.GetMatches()
                        .Where(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2))
                        .Sum(x => x.ScoreAgainst(m.Away).Points),
                    GetMaxSeasonPoints(
                        s.GetMatches()
                            .Count(x => x.HasTeam(m.Home) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(-2)))));*/
            return input;
        }

        public int MarkRound(int round, List<Match> tips)
        {
            var matches = League.GetCurrentSeason().Rounds[round].Matches;
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


        public static String Printlayer(double[] vals)
        {
            var result = vals.Aggregate("{", (current, t) => current + String.Format("{0:N1}, ", t));
            return result + "}";
        }
    }
}
