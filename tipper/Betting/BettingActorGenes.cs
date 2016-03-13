using System;
using System.Collections.Generic;
using System.Linq;
using GeneticAlgorithm;
using Utilities;

namespace Tipper.Betting
{
    public class BettingActorGenes : ActorGenes
    {
        public static int MinNumRules = 1;
        public static int MaxNumRules = 5;
        public List<int> NumRules;
        public static double MinThreshold = 1;
        public static double MaxThreshold = 72;
        public List<List<double>> Thresholds;
        public static double MinWager = 1;
        public static double MaxWager = 20;
        public List<List<double>> Wagers;

        public BettingActorGenes()
        {
            NumRules = new List<int>();
            Thresholds = new List<List<double>>();
            Wagers = new List<List<double>>();
        }

        public BettingActor GenerateBettingActor(int outputs)
        {
            var actor = BettingActor.GetBestGuessBettingActor();
            var numRules = ResolveAttribute(NumRules);

            for (var i = 0; i < numRules; i++)
            {
                var wager = ResolveAttribute(Wagers[i]);
                var threshold = ResolveAttribute(Thresholds[i]);
                var rule = new BettingRule()
                {
                    Priority = i,
                    Wager = wager,
                    Threshold = threshold
                };
                actor.Rules.Add(rule);
            }

            return actor;
        }

        public static BettingActorGenes GenerateRepresentative(List<BettingActor> actors, Random random)
        {
            var representative = new BettingActorGenes();
            if (actors == null || actors.Count == 0)
                return representative;

            //numRules
            var numRulesGeneSet = new ActorGeneSetInt { Min = MinNumRules, Max = MaxNumRules };
            var numRulesPopulationAttributes = actors.Select(a => a.Rules.Count).ToList();
            var numRules = ResolveProbabilities(numRulesGeneSet, numRulesPopulationAttributes);

            //thresholds
            var thresholdsGeneSet = new ActorGeneSetDouble { Min = MinThreshold, Max = MaxThreshold };
            var thresholdsPopulationAttributesList = new List<List<double>>();
            for (int i = MinNumRules; i < MaxNumRules; i++)
            {
                var index = i;
                var thresholdsPopulationAttributes = actors.Where(a => a.Rules.Count > index).Select(a => a.Rules[index].Threshold).ToList();
                thresholdsPopulationAttributesList.Add(thresholdsPopulationAttributes);
            }
            var thresholds = ResolveProbabilities(numRulesGeneSet, thresholdsGeneSet, thresholdsPopulationAttributesList);

            //wagers
            var wagersGeneSet = new ActorGeneSetDouble { Min = MinWager, Max = MaxWager };
            var wagersPopulationAttributesList = new List<List<double>>();
            for (int i = MinNumRules; i < MaxNumRules; i++)
            {
                var index = i;
                var wagersPopulationAttributes = actors.Where(a => a.Rules.Count > index).Select(a => a.Rules[index].Wager).ToList();
                wagersPopulationAttributesList.Add(wagersPopulationAttributes);
            }
            var wagers = ResolveProbabilities(numRulesGeneSet, wagersGeneSet, wagersPopulationAttributesList);

            //Return
            representative.Random = random;
            representative.NumRules = numRules;
            representative.Thresholds = thresholds;
            representative.Wagers = wagers;
            return representative;
        }

        public static BettingActor GenerateRandomActor(Random random)
        {
            var representative = BettingActor.GetBestGuessBettingActor();
            var numRules = random.Next(MinNumRules, MaxNumRules);

            for (var i = 0; i < numRules; i++)
            {
                var wager = Numbery.Normalise(random.NextDouble(), 0, 1, MinWager, MaxWager);
                var threshold = Numbery.Normalise(random.NextDouble(), 0, 1, MinThreshold, MaxThreshold);
                var rule = new BettingRule()
                {
                    Priority = i,
                    Wager = wager,
                    Threshold = threshold
                };
                representative.Rules.Add(rule);
            }

            return representative;
        }
    }
}
