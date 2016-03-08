using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using GeneticArtificialNeuralNetwork;
using Utilities;

namespace Tipper.Betting
{
    public class BettingActor : NetworkActor
    {
        public double Money { get; set; }
        public List<BettingRule> Rules { get; set; }

        public BettingActor() : base(){}

        public BettingActor(DataFacadeGrouped facade, IReadOnlyCollection<int> hiddens, int outputs)
            : base(facade, hiddens, outputs)
        {
        }

        public override void Test(Data data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Facade.SetData(data);
            var subset = Facade.GetData();
            var successes = 0;

            for(var i = 0; i < subset.Inputs().Count; i++)
            {
                var input = subset.DataPoints[i].Inputs;
                var output = Network.Run(input);
                var wager = CalculateWager(output);
                Money -= wager;
                var success = subset.SuccessCondition(output, subset.DataPoints[i].Outputs, false);
                if (success)
                {
                    successes++;
                    var homescore = ((Match) subset.DataPoints[i].Reference).HomeScore().Total();
                    var awayscore = ((Match) subset.DataPoints[i].Reference).AwayScore().Total();
                    var odds = 0.0;
                    if(homescore > awayscore)
                        odds = ((Match)subset.DataPoints[i].Reference).HomeOdds;
                    if (homescore < awayscore)
                        odds = ((Match)subset.DataPoints[i].Reference).AwayOdds;
                    Money += wager*odds;
                }
            }
            SuccessRate = 100 * (double)successes / subset.DataPoints.Count;

            stopwatch.Stop();
            TimeToTest = stopwatch.ElapsedMilliseconds;
        }

        private double CalculateWager(List<double> output)
        {
            var wager = 0.0;
            var phGoals = Numbery.Denormalise(output[0], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(output[1], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(output[2], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(output[3], Util.MaxPoints);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;
            var margin = Math.Abs(phScore - paScore);
            var ordered = Rules.OrderByDescending(x => x.Priority);
            foreach (var rule in ordered)
            {
                if (rule.Scenario(margin))
                    wager = rule.Wager;
            }
            return wager;
        }

        public override double GetFitness()
        {
            return Money;
        }
    }
}
