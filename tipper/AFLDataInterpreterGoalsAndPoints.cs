using System;
using System.Collections.Generic;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class AFLDataInterpreterGoalsAndPoints : AFLDataInterpreter
    {
        #region Inputs
        protected override IEnumerable<double> ExtractInputSetForScore(Match m, List<Match> matches, int term,
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

        protected override IEnumerable<double> ExtractInputSetForWin(Match m, List<Match> matches, int term, Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<double> ExtractInputSetForShots(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            throw new Exception("ExtractInputSetForShots not implemented for goals & points");
        }

        protected override IEnumerable<double> ExtractInputSetForOppotionScore(int term, List<Tuple<Score, Score, DateTime>> homeOppositionScores, List<Tuple<Score, Score, DateTime>> awayOppositionScores)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Outputs
        public override IEnumerable<double> BuildOutputs(Match m)
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

        #region Helpers
        public static AFLDataInterpreterGoalsAndPoints New()
        {
            return new AFLDataInterpreterGoalsAndPoints();
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
            var numSets = original.Count / 4;
            for (var i = 0; i < numSets; i++)
            {
                output.Add(original[i + 2]);
                output.Add(original[i + 3]);
                output.Add(original[i + 0]);
                output.Add(original[i + 1]);
            }
            return output;
        }
        #endregion
    }
}
