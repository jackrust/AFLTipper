using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public abstract class AFLDataInterpreter
    {
        #region inperpretations
        public struct InterpretationSubsets
        {
            public static List<int> DefaultInterpretationSubset = new List<int> { 1, 5, 11, 19, 29 };
        }

        public struct Interpretations
        {
            public static List<List<int>> LatestBestInterpretation = new List<List<int>>
            {
                new List<int> { 1, 5, 11, 19, 29 },
                new List<int> { 1, 5, 11, 19, 29 },
                new List<int> { 1, 5, 11, 19, 29 },
                new List<int> { 1, 5, 11, 19, 29 },
                new List<int> { 5, 11, 19, 29 },
                new List<int> { 1, 5, 11, 19, 29 }
            };

            public static List<List<int>> BespokeLegacyInterpretationV1 = new List<List<int>>
            {
                //False,True,True,True,False,
                //False,False,True,True,True,
                //True,True,False,True,False,
                //False,False,False,True,False,
                //False,True,True,False,True,
                //False,False,True,False,True
                new List<int> {5, 11, 19},
                new List<int> {11, 19, 29},
                new List<int> {1, 5, 19},
                new List<int> {19},
                new List<int> {5, 11, 29},
                new List<int> {11, 29}
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
        #endregion

        #region DataPoint
        public DataPoint BuildDataPoint(List<Match> history, Match m)
        {
            return BuildDataPoint(history, m, Interpretations.DefaultInterpretation);
        }

        public DataPoint BuildDataPoint(List<Match> history, Match m, List<List<int>> inputInpertretation)
        {
            var datapoint = new DataPoint
            {
                Inputs = (BuildInputs(history, m, inputInpertretation)),
                Outputs = BuildOutputs(m).ToList(),
                Reference = m
            };
            return datapoint;
        }
        #endregion

        #region Inputs
        public List<double> BuildInputs(List<Match> history, Match m, List<List<int>> interpretation)
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

        private IEnumerable<double> ExtractTeamScoreInputSet(Match m, List<Match> matches, int term)
        {
            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractGroundScoreInputSet(Match m, List<Match> matches, int term)
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

        private IEnumerable<double> ExtractStateScoreInputSet(Match m, List<Match> matches, int term)
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

        private IEnumerable<double> ExtractDayScoreInputSet(Match m, List<Match> matches, int term)
        {
            Func<Match, bool> homeWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Home));
            Func<Match, bool> awayWherePredicate = (x => x.Date.DayOfWeek == m.Date.DayOfWeek && x.HasTeam(m.Away));
            return ExtractInputSet(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        private IEnumerable<double> ExtractOpponentScoreSet(Match m, List<Match> matches,
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

        private IEnumerable<double> ExtractSharedOpponentScoreSet(Match m, List<Match> matches,
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
                (x => x.HasTeam(m.Home) &&
                      x.HasTeam(recentMatchesAway.Select(y => y.GetOpposition(m.Away)).ToList()));
            Func<Match, bool> awayWherePredicate =
                (x => x.HasTeam(m.Away) && x.HasTeam(recentMatchesHome.Select(y => y.GetOpposition(m.Home)).ToList()));
            return ExtractInputSet(m, matches, longTerm, homeWherePredicate, awayWherePredicate);
        }

        protected abstract IEnumerable<double> ExtractInputSet(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

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
        public abstract IEnumerable<double> BuildOutputs(Match m);
        #endregion

        #region constants
        public static double GetMaxSeasonMargin(double rounds)
        {
            return Util.MaxMargin * rounds;
        }

        public static double GetMaxSeasonTotal(double rounds)
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
        #endregion
    }
}
