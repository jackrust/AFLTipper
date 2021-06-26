using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;
using Cricket;
using Utilities;

namespace Tipper
{
    public abstract class BBLDataInterpreter
    {
        public int DataExpiryDays = 720;

        #region inperpretations
        public struct Interpretations
        {
            //If Interpretation change network will need to change too
            public static List<List<int>> Default = new List<List<int>>
            {
                new List<int> { 1, 2, 3},
                new List<int> { 1, 3, 8, 13 },
                new List<int> { 1, 3, 8, 13 },
                new List<int> { 1, 3, 8, 13 },
                new List<int> { 1, 3, 8, 13 }
            };
        }
        #endregion

        #region DataPoint
        public DataPoint BuildDataPoint(List<Match> history, Match m)
        {
            return BuildDataPoint(history, m, Interpretations.Default);
        }

        public DataPoint BuildDataPoint(List<Match> history, Match m, List<List<int>> inputInpertretation)
        {
            var datapoint = new DataPoint
            {
                Inputs = (BuildInputs(history, m, inputInpertretation)),
                Outputs = BuildOutputs(m).ToList(),
                Reference = m.EffectiveID()
            };
            return datapoint;
        }
        #endregion

        #region Inputs
        public List<double> BuildInputs(List<Match> history, Match m, List<List<int>> interpretation)
        {
            var input = new List<double>();
            
            //V1 - measure by Score

            //Wins By Team
            if (interpretation.Count < 1)
                return input;
            foreach (var term in interpretation[0])
            {
                if(term > 0)
                    input.AddRange(ExtractTeamRecentGamesInputSet(m, history, term, ExtractInputForSpecificWin));//ExtractInputSetForWinningness));
            }//+12, 12

            //Wins By Team
            if (interpretation.Count < 2)
                return input;
            foreach (var term in interpretation[0])
            {
                if (term > 0)
                    input.AddRange(ExtractTeamRecentGamesInputSet(m, history, term, ExtractInputSetForWin));
            }//+12, 24

            //Recent totals By Team
            if (interpretation.Count < 3)
                return input;
            foreach (var term in interpretation[0])
            {
                if (term > 0)
                    input.AddRange(ExtractTeamRecentGamesInputSet(m, history, term, ExtractInputSetForAverage));
            }//+24, 48

            if (interpretation.Count < 4)
                return input;
            foreach (var term in interpretation[0])
            {
                if (term > 0)
                    input.AddRange(ExtractTeamRecentGamesInputSet(m, history, term, ExtractInputSetForStrikeRate));
            }//+24, 72

            
            //Scores By Ground
            if (interpretation.Count < 5)
                return input;
            foreach (var term in interpretation[1])
            {
                if (term > 0)
                    input.AddRange(ExtractGroundInputSet(m, history, term, ExtractInputSetForWin));
            }//2
            
            return input;
        }

        private IEnumerable<double> ExtractTeamRecentGamesInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            Func<Match, bool> homeWherePredicate = (x => x.HasTeam(m.Home) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate = (x => x.HasTeam(m.Away) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }

        
        private IEnumerable<double> ExtractGroundInputSet(Match m, List<Match> matches, int term, Func<Match, List<Match>, int, Func<Match, bool>, Func<Match, bool>, IEnumerable<double>> extrator)
        {
            const int relevantYearsDifference = -12;

            Func<Match, bool> homeWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Home) && x.Date > m.Date.AddYears(relevantYearsDifference) && x.Date > m.Date.AddDays(-DataExpiryDays));
            Func<Match, bool> awayWherePredicate =
                (x =>
                    x.Ground.Equals(m.Ground) && x.HasTeam(m.Away) && x.Date > m.Date.AddYears(relevantYearsDifference) && x.Date > m.Date.AddDays(-DataExpiryDays));
            return extrator(m, matches, term, homeWherePredicate, awayWherePredicate);
        }


        protected abstract IEnumerable<double> ExtractInputSetForWin(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

        protected abstract IEnumerable<double> ExtractInputSetForWinningness(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

        protected abstract IEnumerable<double> ExtractInputForSpecificWin(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

        protected abstract IEnumerable<double> ExtractInputSetForTotal(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

        protected abstract IEnumerable<double> ExtractInputSetForAverage(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate);

        protected abstract IEnumerable<double> ExtractInputSetForStrikeRate(Match m, List<Match> matches, int term,
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

        public static double ExtractInputAverage(List<Match> s, Func<Match, bool> wherePredicate, int takeLength,
            Func<Match, double> sumSelector, Func<double> maxFunc)
        {
            if (s.Where(wherePredicate).Count() == 0)
                return 0.5;

            var value = s
                .Where(wherePredicate)
                .OrderByDescending(x => x.Date)
                .Take(takeLength)
                .Average(sumSelector);
            return Numbery.Normalise(value, maxFunc());
        }

        public static double ExtractInputSpecific(List<Match> s, Func<Match, bool> wherePredicate, int skipLength,
            Func<Match, double> sumSelector, double max)
        {
            var value = s
                .Where(wherePredicate)
                .OrderByDescending(x => x.Date)
                .Skip(skipLength)
                .Take(1)
                .Sum(sumSelector);
            return Numbery.Normalise(value, max);
        }
        #endregion

        #region Outputs
        public abstract IEnumerable<double> BuildOutputs(Match m);
        #endregion

        #region constants
        public static double GetMaxMatches(double matches)
        {
            return matches;
        }
        public static double GetMaxTotal(double matches)
        {
            return 278* matches;//278 from 20 overs
        }

        public static double GetMaxStrikeRate()
        {
            return 14;//278 from 20 overs
        }

        public static double GetMaxAverage()
        {
            return 278;//278 from 20 overs
        }

        #endregion
    }
}
