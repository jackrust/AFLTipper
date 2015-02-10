using System;
using System.Collections.Generic;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    class Program
    {
        //public static Tipper tipper = new Tipper();
        static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            Console.WriteLine("Creating training data...");
            var trainingData = tipper.LearnFromTo(2012, 24 - Tipper.RelevantRoundHistory, 2013, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.LearnFromTo(2013, 24 - Tipper.RelevantRoundHistory, 2014, 24);
            Console.WriteLine("Creating Optimizer...");
            var optimizer = new Optimizer();
            Console.WriteLine("Optimizing...");
            var output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints);
            Console.WriteLine(output);
            Console.Read();
        }

        public static bool SuccessConditionGoalAndPoints(List<double> predicted, List<double> actual)
        {
            var phGoals = Numbery.Denormalise(predicted[0], Util.MAX_GOALS);
            var phPoints = Numbery.Denormalise(predicted[1], Util.MAX_POINTS);
            var paGoals = Numbery.Denormalise(predicted[2], Util.MAX_GOALS);
            var paPoints = Numbery.Denormalise(predicted[3], Util.MAX_POINTS);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MAX_GOALS);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MAX_POINTS);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MAX_GOALS);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MAX_POINTS);
            var ahScore = ahGoals * 6 + ahPoints;
            var aaScore = aaGoals * 6 + aaPoints;

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static bool SuccessConditionWinLoss(List<double> predicted, List<double> actual)
        {

            if (predicted[0] > 0.5 && actual[0] > 0.5)
                return true;
            if (predicted[0] < 0.5 && actual[0] < 0.5)
                return true;
            if (predicted[0] == 0.5 && actual[0] == 0.5)
                return true;
            return false;
        }
    }
}
