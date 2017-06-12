using System;
using System.Collections.Generic;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    public class AFLDataInterpreterTotalSnipped : AFLDataInterpreter
    {
        #region Inputs
        protected override IEnumerable<double> ExtractInputSetForScore(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Total()), GetMaxSeasonTotal),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Total()), GetMaxSeasonTotal),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Total()), GetMaxSeasonTotal),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Total()), GetMaxSeasonTotal)
            };

            return inputSet;
        }

        protected override IEnumerable<double> ExtractInputSetForShots(Match m, List<Match> matches, int term,
            Func<Match, bool> homeWherePredicate, Func<Match, bool> awayWherePredicate)
        {
            var inputSet = new List<double>
            {
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreFor(m.Home).Goals + x.ScoreFor(m.Home).Points), GetMaxSeasonShots),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreFor(m.Away).Goals + x.ScoreFor(m.Away).Points), GetMaxSeasonShots),
                ExtractInput(matches, homeWherePredicate, term, (x => x.ScoreAgainst(m.Home).Goals + x.ScoreAgainst(m.Home).Points), GetMaxSeasonShots),
                ExtractInput(matches, awayWherePredicate, term, (x => x.ScoreAgainst(m.Away).Goals + x.ScoreAgainst(m.Away).Points), GetMaxSeasonShots)
            };

            return inputSet;
        }

        #endregion

        #region Outputs
        public override IEnumerable<double> BuildOutputs(Match match, Numbery.NormalisationMethod normalisationMethod)
        {
            return (new List<double>()
            {
                Numbery.Normalise(match.HomeScore().Total(), Util.MaxReasonableScore, normalisationMethod),
                Numbery.Normalise(match.AwayScore().Total(), Util.MaxReasonableScore, normalisationMethod)
            });
        }

        public override IEnumerable<double> RetrieveOutputs(List<double> result, Numbery.NormalisationMethod normalisationMethod)
        {
            return (new List<double>()
            {
                Numbery.Denormalise(result[0], Util.MaxReasonableScore, normalisationMethod),
                Numbery.Denormalise(result[1], Util.MaxReasonableScore, normalisationMethod)
            });
        }
        #endregion

        #region constants
        public new static double GetMaxSeasonTotal(double rounds)
        {
            //TODO magic number galore
            if (rounds < 1) return 0;
            var a = (1800/23);
            var b = 200 - a;
            return a*rounds + b;
        }
        #endregion

        #region Helpers
        public static AFLDataInterpreterTotal New()
        {
            return new AFLDataInterpreterTotal();
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
