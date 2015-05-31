using System;
using System.Collections.Generic;
using System.Linq;
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
            var loop = true;
            Console.WriteLine("Footy Tipper");
            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                Console.WriteLine(command);
                switch (command.ToUpper())
                {
                    case ("T"):
                        TipNextRound();
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    case ("O"):
                        TestOptimizer();
                        break;
                    case ("E"):
                        Testing();
                        break;
                    case ("?"):
                        ListOptions();
                        break;
                    default:
                        break;
                }
            }

        }

        private static void TipNextRound()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            var date = DateTime.Now;

            Console.WriteLine("Init Neural Network...");
            var trainingData = tipper.LearnFromTo(2010, 0, date);
            Console.WriteLine("Create network...");
            tipper.Net = CreateNetwork(trainingData, 1, 6, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            Console.WriteLine("Tip 2015 round...");
            var tips = tipper.PredictNext(date, true);

            Console.Read();
        }

        public static void ListOptions()
        {
            Console.WriteLine("[T]ip next round");
            Console.WriteLine("[O]ptimise");
            Console.WriteLine("T[E]sting");
            Console.WriteLine("[S]erialize");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("[?] Show options");
        }

        private static void Testing()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            var trainingData1 = new Data();
            var testingData1 = new Data();
            var trainingData2 = new Data();
            var testingData2 = new Data();
            var badData = new Data();
            var goodData = new Data();
            var output = "";
            var name = "";
            var successes = 0;

            goodData.Inputs = new List<List<double>>();
            badData.Inputs = new List<List<double>>();

            goodData.Outputs = new List<List<double>>();
            badData.Outputs = new List<List<double>>();

            trainingData1 = tipper.LearnFromTo(2009, 0, 2012, 24);
            testingData1 = tipper.LearnFromTo(2013, 1, 2013, 24);

            trainingData2 = tipper.LearnFromTo(2013, 1, 2013, 24);
            testingData2 = tipper.LearnFromTo(2014, 1, 2014, 24);

            //Build
            Console.WriteLine("First...");
            Network network = new Network(trainingData1.Inputs[0].Count, new List<int>() { 6 }, trainingData1.Outputs[0].Count);
            name = network.Id;
            network.Train(trainingData1.Inputs, trainingData1.Outputs);
            Network.Save(network);

            successes = testingData1.Inputs.Select(t => network.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.Outputs[i])).Count();
            Console.WriteLine("Normal % = " + ((double)successes / (double)testingData1.Inputs.Count));

            

            //Seperate bad and good data
            for (var i = 0; i < trainingData1.Inputs.Count; i++)
            {
                var result = network.Run(trainingData1.Inputs[i]);
                if (SuccessConditionGoalAndPoints(result, trainingData1.Outputs[i]))
                {
                    goodData.Inputs.Add(trainingData1.Inputs[i]);
                    goodData.Outputs.Add(trainingData1.Outputs[i]);
                }
                else
                {
                    badData.Inputs.Add(trainingData1.Inputs[i]);
                    badData.Outputs.Add(trainingData1.Outputs[i]);
                }

            }

            Console.WriteLine("Good...");
            //Network goodNetwork = new Network(goodData.Inputs[0].Count, new List<int>() { 6 }, goodData.Outputs[0].Count);
            Network goodNetwork = network;
            goodNetwork.Train(goodData.Inputs, goodData.Outputs);
            successes = testingData1.Inputs.Select(t => goodNetwork.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.Outputs[i])).Count();
            Console.WriteLine("Good % = " + ((double)successes / (double)testingData1.Inputs.Count));

            Console.WriteLine("Bad...");
            //Network badNetwork = new Network(badData.Inputs[0].Count, new List<int>() { 6 }, badData.Outputs[0].Count);
            Network badNetwork = network;
            badNetwork.Train(badData.Inputs, badData.Outputs);
            successes = testingData1.Inputs.Select(t => badNetwork.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.Outputs[i])).Count();
            Console.WriteLine("Bad % = " + ((double)successes / (double)testingData1.Inputs.Count));


            /*
            //Test
            successes = testingData1.Inputs.Select(t => network.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.Outputs[i])).Count();
            Console.WriteLine("Unpacked % = " + ((double)successes / (double)testingData1.Inputs.Count));

            //Pack
            Network.Save(network);
            

            //Unpack
            network = Network.Load("0b4d7a36-94ad-4609-a082-aa040e158c6f.txt");
            successes = testingData1.Inputs.Select(t => network.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.Outputs[i])).Count();
            Console.WriteLine("Unpacked % = " + ((double)successes / (double)testingData1.Inputs.Count));
            

            //Retrain
            network.Train(trainingData2.Inputs, trainingData2.Outputs);

            //Test
            successes = testingData2.Inputs.Select(t => network.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData2.Outputs[i])).Count();
            Console.WriteLine("Unpacked % = " + ((double)successes / (double)testingData2.Inputs.Count));

            Network.Save(network);
            */
            Console.Read();
        }

        private static void Tip()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();

            Console.WriteLine("Init Neural Network...");
            var trainingData = tipper.LearnFromTo(2010, 0, 2015, 6);
            Console.WriteLine("Create network...");
            tipper.Net = CreateNetwork(trainingData, 1, 6, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            Console.WriteLine("Tip 2015 round...");
            var tips = tipper.Predict(2015, 7, true);

            Console.Read();
        }

        private static void TestOptimizer()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            Console.WriteLine("Creating Optimizer...");
            var optimizer = new Optimizer();
            var output = "";

            optimizer.LowerLimitLayers = 1;
            optimizer.UpperLimitLayers = 1;
            optimizer.LowerLimitNeuronsInLayer = 1;
            optimizer.UpperLimitNeuronsInLayer = 6;

            Console.WriteLine("2014");

            Console.WriteLine("Creating training data...");
            var trainingDatafull = tipper.LearnFromTo(2008, 0, 2013, 24);
            var trainingData = tipper.LearnFromTo(2008, 0, 2013, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.LearnFromTo(2014, 1, 2014, 24);

            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints, Deconvert);

            Console.WriteLine(output);

            Console.Read();
        }

        public static Network CreateNetwork(Data trainingData, int numLayers, int perLayer, TrainingAlgorithmFactory.TrainingAlgorithmType algorithm)
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
            var phScore = phGoals * 6 +phPoints;
            var paScore = paGoals * 6 +paPoints;

            var ahGoals = Numbery.Denormalise(actual[0], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[1], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
            var ahScore = ahGoals * 6 +ahPoints;
            var aaScore = aaGoals * 6 +aaPoints;

            Console.WriteLine("[{0}, {1} Vs {2}, {3}]", phScore, paScore, ahScore, aaScore);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static List<double> Deconvert(List<double> input)
        {
            var hGoals = Numbery.Denormalise(input[0], Util.MaxGoals);
            var hPoints = Numbery.Denormalise(input[1], Util.MaxPoints);
            var aGoals = Numbery.Denormalise(input[2], Util.MaxGoals);
            var aPoints = Numbery.Denormalise(input[3], Util.MaxPoints);
            return new List<double>(){
                hGoals, hPoints, aGoals, aPoints
            };
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
