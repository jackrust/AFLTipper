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
            var tipperScore = new TipperScore();
            var tipperGoalsPoints = new TipperGoalsPoints();
            var tipper = new Tipper();
            Console.WriteLine("Creating Optimizer...");
            var optimizer = new Optimizer();
            var trainingData = new Data();
            var testingData = new Data();
            var output = "";

            Console.WriteLine("2013");
            Console.WriteLine("Creating training data...");
            trainingData = tipper.LearnFromTo(2009, 0, 2013, 24);
            Console.WriteLine("Creating testing data...");
            testingData = tipper.LearnFromTo(2014, 0, 2014, 24);
            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints);
            Console.WriteLine(output);

            /*Console.WriteLine("2014 - Score");
            Console.WriteLine("Creating training data...");
            trainingData = tipperScore.LearnFromTo(2011, 0, 2013, 24);
            Console.WriteLine("Creating testing data...");
            testingData = tipperScore.LearnFromTo(2014, 0, 2014, 24);
            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionScore);
            Console.WriteLine(output);*/

            /*Console.WriteLine("2014 - Goals points");
            Console.WriteLine("Creating training data...");
            trainingData = tipperGoalsPoints.LearnFromTo(2011, 0, 2013, 24);
            Console.WriteLine("Creating testing data...");
            testingData = tipperGoalsPoints.LearnFromTo(2014, 0, 2014, 24);
            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints);
            Console.WriteLine(output);*/


            //Console.WriteLine("Init Neural Network...");
            //trainingData = tipper.LearnFromTo(2013, 0, 2014, 24);
            //tipper.Net = CreateNetwork(trainingData, 1, 2, Network.TrainingAlgorithm.HoldBestSpiralOut);
            //Console.WriteLine("Tip 2015 round 1...");
            //tipper.Predict(2015, 1, true);
            Console.Read();
        }

        public static Network CreateNetwork(Data trainingData, int numLayers, int perLayer, Network.TrainingAlgorithm algorithm)
        {
            //Create hidden layers
            var hidden = new List<int>();

            for (var i = 0; i < numLayers; i++)
            {
                hidden.Add(perLayer);
            }

            //Create Network
            var network = new Network(trainingData.Inputs[0].Count, hidden, trainingData.Outputs[0].Count);
            //New network with 5 inputs, One hidden layer of 2 neurons, 1 output

            //Train the network
            network.Train(trainingData.Inputs, trainingData.Outputs, algorithm);

            return network;
        }

        public static bool SuccessConditionGoalAndPoints(List<double> predicted, List<double> actual)
        {
            var phGoals = Numbery.Denormalise(predicted[0], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(predicted[1], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(predicted[2], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(predicted[3], Util.MaxPoints);
            var phScore = phGoals * 6 + phPoints;
            var paScore = paGoals * 6 + paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
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

        public static bool SuccessConditionScore(List<double> predicted, List<double> actual)
        {
            var phScore = Numbery.Denormalise(predicted[0], Util.MaxScore);
            var paScore = Numbery.Denormalise(predicted[1], Util.MaxScore);

            var ahScore = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaScore = Numbery.Denormalise(actual[1], Util.MaxScore);

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
