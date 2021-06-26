using System;
using System.Collections.Generic;
using ArtificialNeuralNetwork.DataManagement;
using Cricket;
using Utilities;

namespace Tipper
{
    public class BBLDataInterpreterWin : BBLDataInterpreter
    {
        #region Inputs
        protected override IEnumerable<double> ExtractInputSetForWin(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.Tie() ? 0.5 : (x.WinFor(m.Home) ? 1 : 0)), GetMaxMatches),
                ExtractInput(matches, awayWherePredicate, term, (x => x.Tie() ? 0.5 : (x.WinFor(m.Away) ? 1 : 0)), GetMaxMatches)
            };

            return inputSet;
        }

        protected override IEnumerable<double> ExtractInputForSpecificWin(Match m, List<Match> matches, int skip,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInputSpecific(matches, homeWherePredicate, skip, (x => x.Tie() ? 0.5 : (x.WinFor(m.Home) ? 1.0 : 0.0)), 1.0),
                ExtractInputSpecific(matches, awayWherePredicate, skip, (x => x.Tie() ? 0.5 : (x.WinFor(m.Away) ? 1.0 : 0.0)), 1.0)
            };

            return inputSet;
        }

        //TODO: total is misleading if the winner bats second
        protected override IEnumerable<double> ExtractInputSetForTotal(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Runs), GetMaxTotal),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Runs), GetMaxTotal),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Runs), GetMaxTotal),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Runs), GetMaxTotal)
            };

            return inputSet;
        }

        protected override IEnumerable<double> ExtractInputSetForWinningness(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.WinningnessFor(m.Home)), GetMaxTotal),
                ExtractInput(matches, awayWherePredicate, term, (x => x.WinningnessFor(m.Away)), GetMaxTotal)
            };

            return inputSet;
        }


        protected override IEnumerable<double> ExtractInputSetForAverage(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInputAverage(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Average()), GetMaxAverage),
                ExtractInputAverage(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Average()), GetMaxAverage),
                ExtractInputAverage(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Average()), GetMaxAverage),
                ExtractInputAverage(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Average()), GetMaxAverage)
            };

            return inputSet;
        }

        protected override IEnumerable<double> ExtractInputSetForStrikeRate(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInputAverage(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).StrikeRate()), GetMaxStrikeRate),
                ExtractInputAverage(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).StrikeRate()), GetMaxStrikeRate),
                ExtractInputAverage(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).StrikeRate()), GetMaxStrikeRate),
                ExtractInputAverage(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).StrikeRate()), GetMaxStrikeRate)
            };

            return inputSet;
        }
        #endregion

        #region Outputs
        public override IEnumerable<double> BuildOutputs(Match m)
        {
            return (new List<double>()
            {
                m.Tie() ? 0.5 : (m.HomeWin() ? 1 : 0),
                m.Tie() ? 0.5 : (m.AwayWin() ? 1 : 0)
            });
        }
        #endregion

        #region Helpers
        public static BBLDataInterpreterWin New()
        {
            return new BBLDataInterpreterWin();
        }

        //Intended to be used to minimise the amplification of Home Vs Away
        public static DataPoint Invert(DataPoint original)
        {
            var output = new DataPoint
            {
                Inputs = Invert(original.Inputs),
                Outputs = Invert(original.Outputs),
                Reference = original.Reference
            };
            return output;
        }

        public static List<double> Invert(List<double> original)
        {
            var output = new List<double>();
            var numSets = original.Count / 2;
            for (var i = 0; i < numSets; i++)
            {
                output.Add(original[i + 1]);
                output.Add(original[i + 0]);
            }
            return output;
        }
        #endregion
    }
}
