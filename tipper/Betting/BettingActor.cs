using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using GeneticAlgorithm;
using GeneticArtificialNeuralNetwork;
using Utilities;

namespace Tipper.Betting
{
    public class BettingActor : Actor
    {
        public double Money { get; set; }
        public List<BettingRule> Rules { get; set; }
        public NetworkActor NetworkActor;
        public long TimeToTest;

        public BettingActor(NetworkActor networkActor)
        {
            Rules = new List<BettingRule>();
            NetworkActor = networkActor;
        }

        public void Train(Data data)
        {
            NetworkActor.Train(data);
        }

        public void Test(Data data)
        {
            var successes = 0;
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            NetworkActor.Facade.SetData(data);
            var subset = NetworkActor.Facade.GetData();

            for(var i = 0; i < subset.Inputs().Count; i++)
            {
                var input = subset.DataPoints[i].Inputs;
                var output = NetworkActor.Network.Run(input);
                var wager = CalculateWager(output);
                Money -= wager;
                var success = subset.SuccessCondition(output, subset.DataPoints[i].Outputs, null);
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
            //Console.WriteLine("successes = " + (double)successes / (double)subset.Inputs().Count);
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
            var ordered = Rules.OrderByDescending(x => x.Priority).ToList();
            foreach (var rule in ordered)
            {
                wager = rule.Scenario(margin);
            }
            return wager;
        }

        public override double GetFitness()
        {
            return Money;
        }

        public static BettingActor GetBestGuessBettingActor()
        {
            return new BettingActor(null);//new BettingActor(NetworkActor.BestGuessNetworkActor());
        }
    }
}
