using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class AFLDataInterpreter
    {
        public struct InterpretationSubsets
        {
            public static List<int> DefaultInterpretationSubset = new List<int> { 1, 5, 11, 19, 29 };
        }

        public struct Interpretations
        {
            public static List<List<int>> LatestBestInterpretation = new List<List<int>>
            {
                new List<int> {1},
                new List<int>(),
                new List<int> {5, 29},
                new List<int> {29},
                new List<int> {29},
                new List<int> {1, 5, 19}
            };

            public static List<List<int>> BespokeLegacyInterpretation = new List<List<int>>
            {
                new List<int> {1, 19},
                new List<int> {5, 19},
                new List<int> {29},
                new List<int> {5, 11, 19},
                new List<int> {11},
                new List<int> {1, 19}
            };

            public static List<List<int>> DefaultInterpretation = new List<List<int>>
            {
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset,
                InterpretationSubsets.DefaultInterpretationSubset
            };
        }
        
        

        #region DataPoint
        public static DataPoint BuildDataPoint(List<Match> history, Match m)
        {
            return BuildDataPoint(history, m, Interpretations.DefaultInterpretation);
        }

        public static DataPoint BuildDataPoint(List<Match> history, Match m, List<List<int>> inputInpertretation)
        {
            var datapoint = new DataPoint
            {
                Inputs = (BuildInputs(history, m, inputInpertretation)),
                Outputs = BuildOutputs(m),
                Reference = m
            };
            return datapoint;
        }
        #endregion

        #region Inputs
        public static List<double> BuildInputs(List<Match> history, Match m, List<List<int>> interpretation)
        {
            var input = new List<double>();

            //Scores By Team
            foreach (var term in interpretation[0])
            {
                input.AddRange(ExtractTeamScoreInputSet(m, history, term));
            }

            //Scores By Ground
            foreach (var term in interpretation[1])
            {
                input.AddRange(ExtractGroundScoreInputSet(m, history, term));
            }

            //Scores By State longerTerm
            foreach (var term in interpretation[2])
            {
                input.AddRange(ExtractStateScoreInputSet(m, history, term));
            }

            //Scores by Day
            foreach (var term in interpretation[3])
            {
                input.AddRange(ExtractDayScoreInputSet(m, history, term));
            }

            //Recent Opponents
            foreach (var term in interpretation[4])
            {
                input.AddRange(ExtractOpponentScoreSet(m, history, term));
            }

            //Recent Shared Opponents
            foreach (var term in interpretation[5])
            {
                input.AddRange(ExtractSharedOpponentScoreSet(m, history, term));
            }

            return input;
        }

        private static IEnumerable<double> ExtractTeamScoreInputSet(Match m, List<Match> matches, int term)
        {
            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractGroundScoreInputSet(Match m, List<Match> matches, int term)
        {
            const int relevantYearsDifference = -12;

            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractStateScoreInputSet(Match m, List<Match> matches, int term)
        {
            const int relevantYearsDifference = -12;
            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Home) &&
                    x.Date > m.Date.AddYears(relevantYearsDifference));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.State.Equals(m.Ground.State) && x.HasTeam(m.Away) &&
                    x.Date > m.Date.AddYears(relevantYearsDifference));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractDayScoreInputSet(Match m, List<Match> matches, int term)
        {
            Func<Match, bool> homeWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractOpponentScoreSet(Match m, List<Match> matches,
            int term)
        {
            const int numOpponents = 6;
            var recentOpponentsHome =
                matches.Where(mtch => mtch.HasTeam(m.Home))
                    .OrderByDescending(x => x.Date)
                    .Take(numOpponents)
                    .Select(mtch => mtch.GetOpposition(m.Home))
                    .ToList();
            var recentOpponentsAway =
                matches.Where(mtch => mtch.HasTeam(m.Away))
                    .OrderByDescending(x => x.Date)
                    .Take(numOpponents)
                    .Select(mtch => mtch.GetOpposition(m.Away))
                    .ToList();

            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(recentOpponentsHome) && !x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(recentOpponentsAway) && !x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractSharedOpponentScoreSet(Match m, List<Match> matches,
            int longTerm)
        {
            const int numOpponents = 15;
            var recentMatchesHome =
                matches.Where(mtch => mtch.HasTeam(m.Home) && !mtch.HasTeam(m.Away))
                    .OrderByDescending(mtch => mtch.Date)
                    .Take(numOpponents)
                    .ToList();
            var recentMatchesAway =
                matches.Where(mtch => mtch.HasTeam(m.Away) && !mtch.HasTeam(m.Home))
                    .OrderByDescending(mtch => mtch.Date)
                    .Take(numOpponents)
                    .ToList();

            Func<Match, bool> homeWherePredicate =
                (x => x.HasTeam(m.Home) && x.HasTeam(recentMatchesAway.Select(y => y.GetOpposition(m.Away)).ToList()));
            Func<Match, bool> awayWherePredicate =
                (x => x.HasTeam(m.Away) && x.HasTeam(recentMatchesHome.Select(y => y.GetOpposition(m.Home)).ToList()));
            return ExtractInputSet(m, matches, longTerm, homeWherePredicate, awayWherePredicate);
        }

        private static IEnumerable<double> ExtractInputSet(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Points), GetMaxSeasonPoints),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Points), GetMaxSeasonPoints),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Points), GetMaxSeasonPoints),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Goals), GetMaxSeasonGoals),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Points), GetMaxSeasonPoints)
            };

            return inputSet;
        }

        public static double ExtractInput(List<Match> s, Func<Match, bool> wherePredicate, int takeLength,
            Func<Match, double> sumSelector, Func<double, double> maxFunc)
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
        #endregion

        #region Outputs
        public static List<double> BuildOutputs(Match m)
        {
            return (new List<double>()
            {
                Numbery.Normalise(m.HomeScore().Goals, Util.MaxGoals),
                Numbery.Normalise(m.HomeScore().Points, Util.MaxPoints),
                Numbery.Normalise(m.AwayScore().Goals, Util.MaxGoals),
                Numbery.Normalise(m.AwayScore().Points, Util.MaxPoints)
            });
        }
        #endregion

        #region constants
        public static double GetMaxSeasonGoals(double rounds)
        {
            return Util.MaxGoals * rounds;
        }

        public static double GetMaxSeasonPoints(double rounds)
        {
            return Util.MaxPoints * rounds;
        }
        #endregion
    }
}
