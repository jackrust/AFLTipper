using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Utilities;

namespace Tipper
{
    class Program
    {
        //public static Tipper tipper = new Tipper();
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            var loop = true;
            Console.WriteLine("Footy Tipper");
            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                switch (command.ToUpper())
                {
                    case ("T"):
                        TipNextRound();
                        break;
                    case ("T7"):
                        Tip();
                        break;
                    case ("B"):
                        TestBrain();
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    case ("O"):
                        TestOptimizer();
                        break;
                    case ("L"):
                        RunLulu();
                        break;
                    case ("E"):
                        Testing();
                        break;
                    case ("?"):
                        ListOptions();
                        break;
                    case ("F"):
                        TestFunction();
                        break;
                    default:
                        break;
                }
            }

        }

        //Network(1,1,50, Lulu) % = 0.670748299319728
        private static void RunLulu()
        {
            var loop = true;
            var date = DateTime.Now;//.Subtract(new TimeSpan(5, 0, 0, 0));
            Console.WriteLine("Hi, I'm Lulu");


            Console.WriteLine("I'm just looking up the latest AFL stats...");
            //TODO: Get Latest Results from FinalSirenAPI

            Console.WriteLine("Waking up now...");
            //TODO: Check if ANN exists forcreating a new one
            var tipper = new Tipper();
            var data = tipper.LearnFromTo(2008, 0, date);
            data.SuccessCondition = SuccessConditionGoalAndPointsPrint;
            var testingData = new Data() { DataPoints = data.DataPoints.GetRange(0, data.DataPoints.Count / 2) };
            var trainingData = new Data() { DataPoints = data.DataPoints.GetRange(data.DataPoints.Count / 2, data.DataPoints.Count / 2) };

            var network = new Network(trainingData.DataPoints[0].Inputs.Count, new List<int>() {1},
                trainingData.DataPoints[0].Outputs.Count);
            if (Filey.FindFile("Lulu/", "Lulu.ann") != null)
            {
                network = Network.Load("Lulu/Lulu.ann");
            }
            else
            {
                network = new Network(trainingData.DataPoints[0].Inputs.Count, new List<int>() {1},
                    trainingData.DataPoints[0].Outputs.Count);
                network.MaxEpochs = 50;
                network.Id = "Lulu";
                network.Directory = "Lulu/";
                network.Train(trainingData.Inputs(), trainingData.Outputs());
            }

            var successes = testingData.Inputs().Select(t => network.Run(t)).Where((result, i) => data.SuccessCondition(result, testingData.DataPoints[i].Outputs, false)).Count();
            Console.WriteLine("Network({0},{1},{2}, {3}) % = {4}", network.HLayers.Count, network.HLayers[0].Count, network.MaxEpochs, network.Id, ((double)successes / (double)testingData.DataPoints.Count));

            Console.WriteLine("Going through and reestimating ");
            //TODO: restimate Seasson

            Console.WriteLine("Tipping...");
            tipper.Net = network;
            //tipper.PredictNext(date, true);

            //For each round until the end of the season
            foreach (var r in tipper.League.GetCurrentSeason().Rounds.Where(r => r.Matches.TrueForAll(m => m.Date > date)))
            {
                Console.WriteLine("");
                Console.WriteLine("Round{0}...",r.Number);
                tipper.Predict(r, true);
            }
        }

        private static void TestBrain()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            Console.WriteLine("Creating Optimizer...");
            var brain = new Brain.Brain();

            Console.WriteLine("2014");

            Console.WriteLine("Creating training data...");
            var data = tipper.LearnFromTo(2008, 0, 2015, 13);
            data.SuccessCondition = SuccessConditionGoalAndPointsPrint;
            Console.WriteLine("Creating testing data...");

            Console.WriteLine("Mulling...");
            brain.Data = data;
            var output = brain.Mull();
            Network.Save(output);

            Console.WriteLine("Best...");
            Console.WriteLine(output.HLayers.Count);
            Console.WriteLine(output.HLayers[0].Count);



            Console.Read();
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
            tipper.Net = Network.CreateNetwork(trainingData, 1, 6, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            Console.WriteLine("Tip 2015 round...");
            var tips = tipper.PredictNext(date, true);

            Console.Read();
        }

        public static void ListOptions()
        {
            Console.WriteLine("[T]ip next round");
            Console.WriteLine("[B]rain");
            Console.WriteLine("[O]ptimise");
            Console.WriteLine("T[E]sting");
            Console.WriteLine("[S]erialize");
            Console.WriteLine("[F]unction");
            Console.WriteLine("[L]ulu");
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

            trainingData1 = tipper.LearnFromTo(2009, 0, 2012, 24);
            testingData1 = tipper.LearnFromTo(2013, 1, 2013, 24);

            trainingData2 = tipper.LearnFromTo(2013, 1, 2013, 24);
            testingData2 = tipper.LearnFromTo(2014, 1, 2014, 24);

            //Build
            Console.WriteLine("First...");
            Network network = new Network(trainingData1.DataPoints.Count, new List<int>() { 6 }, trainingData1.DataPoints[0].Outputs.Count);
            name = network.Id;
            network.Train(trainingData1.Inputs(), trainingData1.Outputs());
            Network.Save(network);

            successes = testingData1.Inputs().Select(t => network.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.DataPoints[i].Outputs)).Count();
            Console.WriteLine("Normal % = " + ((double)successes / (double)testingData1.DataPoints.Count));

            

            //Seperate bad and good data
            for (var i = 0; i < trainingData1.DataPoints.Count; i++)
            {
                var result = network.Run(trainingData1.DataPoints[i].Inputs);
                if (SuccessConditionGoalAndPoints(result, trainingData1.DataPoints[i].Outputs))
                {
                    goodData.DataPoints.Add(trainingData1.DataPoints[i]);
                }
                else
                {
                    badData.DataPoints.Add(trainingData1.DataPoints[i]);
                }

            }

            Console.WriteLine("Good...");
            //Network goodNetwork = new Network(goodData.Inputs[0].Count, new List<int>() { 6 }, goodData.Outputs[0].Count);
            Network goodNetwork = network;
            goodNetwork.Train(goodData.Inputs(), goodData.Outputs());
            successes = testingData1.Inputs().Select(t => goodNetwork.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.DataPoints[i].Outputs)).Count();
            Console.WriteLine("Good % = " + ((double)successes / (double)testingData1.DataPoints.Count));

            Console.WriteLine("Bad...");
            //Network badNetwork = new Network(badData.Inputs[0].Count, new List<int>() { 6 }, badData.Outputs[0].Count);
            Network badNetwork = network;
            badNetwork.Train(badData.Inputs(), badData.Outputs());
            successes = testingData1.Inputs().Select(t => badNetwork.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.DataPoints[i].Outputs)).Count();
            Console.WriteLine("Bad % = " + ((double)successes / (double)testingData1.DataPoints.Count));


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
            //var trainingData = tipper.LearnFromTo(2010, 0, 2015, 6);
            Console.WriteLine("Create network...");
            //tipper.Net = CreateNetwork(trainingData, 1, 6, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            //tipper.Net = Network.Load("Network/504a8765-2855-4348-be32-9482d7b537ca.ann"); //1-1
            //tipper.Net = Network.Load("Network/f8fb015d-82db-4512-a95b-07daa543d9ef.ann");//1-5
            //tipper.Net = Network.Load("Network/03ab9e06-f43e-46a7-98b7-84bb6d3d880c.ann");//1-1
            tipper.Net = Network.Load("Network/9db3e13c-5626-45ba-902a-111dd0d195ed.ann");//1-1 66.69
            tipper.Net = Network.Load("Network/cd3879de-bec4-4c5c-9827-5118bd05e87b.ann");//1-1 67.74

            Console.WriteLine("Tip 2015 round...");
            var tips = tipper.Predict(2015, 15, true);

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
            optimizer.UpperLimitNeuronsInLayer = 7;
            //optimizer.LowerLimitEpochs = 50;
            //optimizer.LowerLimitEpochs = 50;

            Console.WriteLine("2014");

            Console.WriteLine("Creating training data...");
            var trainingData = tipper.LearnFromTo(2012, 0, 2014, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.LearnFromTo(2008, 0, 2011, 24);

            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints, Deconvert);

            Console.WriteLine(output);

            Console.Read();
        }

        #region TestFunction
        private static void TestFunction()
        {
            var tipper = new Tipper();
            var successes = 0;

            //var testingData1 = tipper.LearnFromTo(2008, 0, 2010, 24);
            var testingData1 = tipper.LearnFromTo(2015, 0, 2015, 11);

            //Build
            //Network network = Network.Load("Network/504a8765-2855-4348-be32-9482d7b537ca.ann");//1-1
            //Network network = Network.Load("Network/f8fb015d-82db-4512-a95b-07daa543d9ef.ann");//1-5
            Network network = Network.Load("Network/03ab9e06-f43e-46a7-98b7-84bb6d3d880c.ann");//1-1
            successes = testingData1.Inputs().Select(t => network.Run(t)).Where((result, i) => SuccessConditionGoalAndPoints(result, testingData1.DataPoints[i].Outputs)).Count();
            Console.WriteLine("Normal % = " + ((double)successes / (double)testingData1.DataPoints.Count));



            /*var function = ExtractFunction(network);
            var ins = testingData1.DataPoints[0].Inputs;
            var outs = function(ins);
            foreach (var o in outs)
            {
                Console.WriteLine(o);
            }
            Console.WriteLine("End");*/
        }

        public static Func<List<double>, List<double>> ExtractFunction(Network network)
        {

            Func<List<double>, List<double>> function = (x => getAllOutputs(network, x));
            return function;
        }

        public static List<double> getAllOutputs(Network network, List<double> x)
        {
            int numOutputs = network.ONeurons.Count;
            var outputs = new List<double>();

            for (var i = 0; i < numOutputs; i++)
            {
                var getOutputFunction = CreateOutputFunction(network, network.ONeurons[i]);
                outputs.Add(getOutputFunction(x));
            }
            return outputs;
        }

        public static Func<List<double>, double> CreateOutputFunction(Network network, Dynamic output)
        {
            var pathways = network.GetPathways(output);
            //Func<List<double>, double> function = (x => getAllOutputs(network, x));
            return
                (xs =>
                    xs.Sum(
                        x =>
                            pathways.Aggregate(0.0,
                                (pcurrent, p) => pcurrent + p.Weightings.Aggregate(1.0, (wcurrent, w) => wcurrent * w) * x)));
        }

        public static double sumAll(List<double> all)
        {
            return all.Sum();
        }

        public static double produceAll(List<double> all)
        {
            return all.Aggregate(1.0, (current, a) => current * a);
        }
        #endregion


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

            //Console.WriteLine("[{0}, {1} Vs {2}, {3}]", phScore, paScore, ahScore, aaScore);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static bool SuccessConditionGoalAndPointsPrint(List<double> predicted, List<double> actual, bool print)
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

            if(print)
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
