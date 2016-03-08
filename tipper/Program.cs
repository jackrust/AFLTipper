using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using GeneticArtificialNeuralNetwork;
using Tipper.Betting;
using Utilities;

namespace Tipper
{
    class Program
    {
        #region UI
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException("args");
            var loop = true;
            Console.WriteLine("Footy Tipper");
            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case("B"):
                        GeneticBettingAlgorithmTest();
                        break;
                    case ("F"):
                        TestFunction();
                        break;
                    case ("G"):
                        GeneticAlgorithmTest();
                        break;
                    case ("L"):
                        RunLulu();
                        break;
                    case ("N"):
                        TipNextRound();
                        break;
                    case ("O"):
                        TestOptimizer();
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    case ("S"):
                        TipSpecific();
                        break;
                    case ("T"):
                        Testing();
                        break;
                    case ("?"):
                        ListOptions();
                        break;
                }
            }

        }

        public static void ListOptions()
        {
            Console.WriteLine("[B]et season");
            Console.WriteLine("[F]unction");
            Console.WriteLine("[G]enetic algorithm");
            Console.WriteLine("[L]ulu");
            Console.WriteLine("Tip [N]ext round");
            Console.WriteLine("[O]ptimise");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("Tip [S]pecific");
            Console.WriteLine("[T]esting");
            Console.WriteLine("[?] Show options");
        }
        #endregion

        #region Genetic Betting Algorithm
        private static void GeneticBettingAlgorithmTest()
        {
            const int idealPopulation = 15;
            const int matchesInSeason = 196;
            const int maxLoops = 4;

            //Load inputs for full full Dataset
            Console.WriteLine("Creating Tipper, Dataset etc.");
            var guid = Guid.NewGuid().ToString();
            var tipper = new Tipper();
            var data = tipper.BuildFullDataSet();

            //Total inputs: 8 * 5 * 6 = 240
            Console.WriteLine("Creating list of actors...");

            //Create first population
            var actors = new List<BettingActor>();

            Console.WriteLine("Starting generational loop...");
            //Repeat a bunch of times
            var loop = 0;
            while (loop < maxLoops)
            {
                Console.WriteLine("Stating new generation...");
                
                loop++;

                Repopulate(actors, idealPopulation, loop);

                foreach (var actor in actors)
                {
                    
                    var datatrain = new Data();
                    var datatest = new Data();
                    datatrain.SuccessCondition = SuccessConditionGoalAndPointsPrint;
                    datatest.SuccessCondition = SuccessConditionGoalAndPointsPrint;

                    const int numScenarios = 4;
                    var modulo = loop % numScenarios;
                    //0 = (start of 2007 season, end of 2013 season)
                    //1 = (start of 2007 season, end of 2012 season)
                    //2 = (start of 2007 season, end of 2011 season)
                    //3 = (start of 2007 season, end of 2010 season)
                    var endTrainingSet = data.DataPoints.Count - ((2+modulo)*matchesInSeason);

                    datatrain.DataPoints = data.DataPoints.GetRange(0, endTrainingSet);
                    datatest.DataPoints = data.DataPoints.GetRange(endTrainingSet, matchesInSeason);

                    //Train population
                    actor.Train(datatrain);
                    //Test population
                    actor.Test(datatest);

                    Console.WriteLine("Actor has a success rate of {0}%", actor.GetFitness());
                    
                }
                //Save all

                //Sort Best to worst
                actors.Sort((x, y) => y.GetFitness().CompareTo(x.GetFitness()));
                SaveActors(actors, guid, loop);
                SaveLog(actors, guid, loop);
                //cull the weakest
                actors.RemoveRange(actors.Count / 3, (actors.Count * 2 / 3));

            }
            Console.WriteLine("Actor {0} has a success rate of {1}%", 0, actors[0].GetFitness());
        }

        public bool BettingRuleSimple(double margin)
        {
            return margin > 10;
        }

        public static List<BettingActor> Repopulate(List<BettingActor> actors, int targetPopulation, int generation)
        {
            const int minPopulation = 4;
            const int numOutputs = 4;
            var count = actors.Count;
            var random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute);

            if (count == 0)
            {
                var bettingActor = GetBestGuessBettingActor();
                bettingActor.Rules = new List<BettingRule>()
                {
                    new BettingRule()
                    {
                        Priority = 1,
                        Wager = 1,
                        Scenario = (x) => x > 10
                    }
                };
                actors.Add(bettingActor);
            }
            if (count < minPopulation)
            {
                for (var i = count; i < targetPopulation; i++)
                {
                    //var actorGenes = BettingActorGenes.GenerateRandomRepresentative(random);
                    //actors.Add(actorGenes.GenerateActor(numOutputs));
                    var bettingActor = GetBestGuessBettingActor();
                    bettingActor.Rules = new List<BettingRule>()
                    {
                        new BettingRule()
                        {
                            Priority = 1,
                            Wager = 1,
                            Scenario = (x) => x > 10
                        }
                    };
                    actors.Add(bettingActor);
                }
            }
            else
            {
                var actorGenes = BettingActorGenes.GenerateRepresentative(actors, random);
                for (var i = count; i < targetPopulation; i++)
                {
                    //actors.Add(actorGenes.GenerateActor(numOutputs));
                    var bettingActor = GetBestGuessBettingActor();
                    bettingActor.Rules = new List<BettingRule>()
                    {
                        new BettingRule()
                        {
                            Priority = 1,
                            Wager = 1,
                            Scenario = (x) => x > 10
                        }
                    };
                    actors.Add(bettingActor);
                }
            }

            foreach (var actor in actors)
            {
                actor.Generations.Add(generation);
                actor.RefreshNetwork();
            }

            return actors;
        }

        private static BettingActor GetBestGuessBettingActor()
        {
            const int outputs = 4;
            const int hiddens = 1;
            var controlFacade = new DataFacadeGrouped();
            controlFacade.SetMask(new List<bool>
                {
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false
                });

            var actor = new BettingActor(controlFacade, new List<int> { hiddens }, outputs);
            return actor;
        }

        private static void SaveActors(List<BettingActor> actors, string guid, int loop)
        {
            var str = "";
            foreach (var actor in actors)
            {
                Network.Save(actor.Network);
            }
            foreach (var actor in actors)
            {
                str += "<actor>\n";
                str += actor.Stringify() + "\n";
                str += "</actor>\n";
            }

            Filey.Save(str, "GeneticArtificialNeuralNetwork/" + guid + "-" + loop + ".gann");
        }

        private static void SaveLog(List<BettingActor> actors, string guid, int loop)
        {
            var filename = "GeneticArtificialNeuralNetworkLog/" + guid + "-" + loop + ".gann";
            var str = Filey.Load(filename);
            str += "===============" + "\n";
            str += "Generation:    " + loop + "\n";
            str += "===============" + "\n";
            foreach (var actor in actors)
            {
                str += "Actor:         " + actor.Name + "\n";
                str += "Money:   " + actor.GetFitness() + "\n";
                str += "Generations:   " + string.Join(",", actor.Generations.Select(x => x.ToString()).ToArray()) + "\n";
                str += "Subset:        " + string.Join(",", actor.Facade.GetMask().Select(x => x.ToString()).ToArray()) + "\n";
                str += "Time to train: " + actor.TimeToTrain + "\n";
                str += "Time to test:  " + actor.TimeToTest + "\n";
                str += "...\n";
            }

            Filey.Save(str, filename);
        }
        #endregion

        #region Genetic Algorithm
        private static void GeneticAlgorithmTest()
        {
            const int idealPopulation = 15;
            const int matchesInSeason = 196;
            const int maxLoops = 4;

            //Load inputs for full full Dataset
            Console.WriteLine("Creating Tipper, Dataset etc.");
            var guid = Guid.NewGuid().ToString();
            var tipper = new Tipper();
            var data = tipper.BuildFullDataSet();

            //Total inputs: 8 * 5 * 6 = 240
            Console.WriteLine("Creating list of actors...");

            //Create first population
            var actors = new List<NetworkActor>();

            Console.WriteLine("Starting generational loop...");
            //Repeat a bunch of times
            var loop = 0;
            while (loop < maxLoops)
            {
                Console.WriteLine("Stating new generation...");

                loop++;

                Repopulate(actors, idealPopulation, loop);

                foreach (var actor in actors)
                {

                    var datatrain = new Data();
                    var datatest = new Data();
                    datatrain.SuccessCondition = SuccessConditionGoalAndPointsPrint;
                    datatest.SuccessCondition = SuccessConditionGoalAndPointsPrint;

                    const int numScenarios = 4;
                    var modulo = loop % numScenarios;
                    //0 = (start of 2007 season, end of 2013 season)
                    //1 = (start of 2007 season, end of 2012 season)
                    //2 = (start of 2007 season, end of 2011 season)
                    //3 = (start of 2007 season, end of 2010 season)
                    var endTrainingSet = data.DataPoints.Count - ((2 + modulo) * matchesInSeason);

                    datatrain.DataPoints = data.DataPoints.GetRange(0, endTrainingSet);
                    datatest.DataPoints = data.DataPoints.GetRange(endTrainingSet, matchesInSeason);

                    //Train population
                    actor.Train(datatrain);
                    //Test population
                    actor.Test(datatest);

                    Console.WriteLine("Actor has a success rate of {0}%", actor.GetFitness());

                }
                //Save all

                //Sort Best to worst
                actors.Sort((x, y) => y.GetFitness().CompareTo(x.GetFitness()));
                SaveActors(actors, guid, loop);
                SaveLog(actors, guid, loop);
                //cull the weakest
                actors.RemoveRange(actors.Count / 3, (actors.Count * 2 / 3));

            }
            Console.WriteLine("Actor {0} has a success rate of {1}%", 0, actors[0].GetFitness());
        }

        public static List<NetworkActor> Repopulate(List<NetworkActor> actors, int targetPopulation, int generation)
        {
            const int minPopulation = 4;
            const int numOutputs = 4;
            var count = actors.Count;
            var random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute);

            if (count == 0)
            {
                actors.Add(GetBestGuessActor());
            }
            if (count < minPopulation)
            {
                for (var i = count; i < targetPopulation; i++)
                {
                    var actorGenes = ActorGenes.GenerateRandomRepresentative(random);
                    actors.Add(actorGenes.GenerateActor(numOutputs));
                }
            }
            else
            {
                var actorGenes = ActorGenes.GenerateRepresentative(actors, random);
                for (var i = count; i < targetPopulation; i++)
                {
                    actors.Add(actorGenes.GenerateActor(numOutputs));
                }
            }

            foreach (var actor in actors)
            {
                actor.Generations.Add(generation);
                actor.RefreshNetwork();
            }

            return actors;
        }

        private static NetworkActor GetBestGuessActor()
        {
            const int outputs = 4;
            const int hiddens = 1;
            var controlFacade = new DataFacadeGrouped();
            controlFacade.SetMask(new List<bool>
                {
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false,
                    true, false, false, true, false
                });

            var actor = new NetworkActor(controlFacade, new List<int> { hiddens }, outputs);
            return actor;
        }

        private static void SaveActors(List<NetworkActor> actors, string guid, int loop)
        {
            var str = "";
            foreach (var actor in actors)
            {
                Network.Save(actor.Network);
            }
            foreach (var actor in actors)
            {
                str += "<actor>\n";
                str += actor.Stringify() + "\n";
                str += "</actor>\n";
            }

            Filey.Save(str, "GeneticArtificialNeuralNetwork/" + guid + "-" + loop + ".gann");
        }

        private static void SaveLog(List<NetworkActor> actors, string guid, int loop)
        {
            var filename = "GeneticArtificialNeuralNetworkLog/" + guid + "-" + loop + ".gann";
            var str = Filey.Load(filename);
            str += "===============" + "\n";
            str += "Generation:    " + loop + "\n";
            str += "===============" + "\n";
            foreach (var actor in actors)
            {
                str += "Actor:         " + actor.Name + "\n";
                str += "SuccessRate:   " + actor.GetFitness() + "\n";
                str += "Generations:   " + string.Join(",", actor.Generations.Select(x => x.ToString()).ToArray()) + "\n";
                str += "Subset:        " + string.Join(",", actor.Facade.GetMask().Select(x => x.ToString()).ToArray()) + "\n";
                str += "Time to train: " + actor.TimeToTrain + "\n";
                str += "Time to test:  " + actor.TimeToTest + "\n";
                str += "...\n";
            }

            Filey.Save(str, filename);
        }
        #endregion

        #region Lulu
        //Network(1,1,50, Lulu) % = 0.670748299319728
        private static void RunLulu()
        {
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

            Network network;
            if (Filey.FindFile("Lulu/", "Lulu.ann") != null)
            {
                network = Network.Load("Lulu/Lulu.ann");
            }
            else
            {
                network = new Network(trainingData.DataPoints[0].Inputs.Count, new List<int>() {1},
                    trainingData.DataPoints[0].Outputs.Count) {MaxEpochs = 50, Id = "Lulu", Directory = "Lulu/"};
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
        #endregion

        #region Tip next round
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
            Console.WriteLine("..." + tips[0].HomeScore());
            Console.Read();
        }
        #endregion

        #region Tip
        private static void TipSpecific()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();

            Console.WriteLine("Init Neural Network...");
            Console.WriteLine("Create network...");
            //tipper.Net = Network.Load("Network/9db3e13c-5626-45ba-902a-111dd0d195ed.ann");//1-1 66.69
            //tipper.Net = Network.Load("Network/cd3879de-bec4-4c5c-9827-5118bd05e87b.ann");//1-1 67.74
            tipper.Net = Network.Load("Network/f2ee8f0c-c3ed-4209-8e89-9602b68288e8.ann");//1-1 67.74

            Console.WriteLine("Tip 2015 round...");
            var tips = tipper.Predict(2015, 1, true);

            Console.Read();
        }
        #endregion

        #region TipSeason
        private static void TipBetSeason()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            var money = 200.00;

            Console.WriteLine("Init Neural Network...");
            Network network = new Network(88, new List<int>{1}, 4);
            Console.WriteLine("Creating training data...");
            var trainingData = tipper.LearnFromTo(2009, 0, 2011, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.LearnFromTo(2015, 0, 2015, 24);

            Console.WriteLine("Training Neural Network...");
            network.Train(trainingData.Inputs(), trainingData.Outputs());
            
            Console.WriteLine("Testing Neural Network...");
            var successes = new List<bool>();
            for (var i = 0; i < testingData.Outputs().Count; i++)
            {
                var round = (i/8)-1;
                Console.WriteLine("Match: " + ((Match)testingData.DataPoints[i].Reference).Home.ApiName + " vs " + ((Match)testingData.DataPoints[i].Reference).Away.ApiName);
                Console.WriteLine("Odds: " + ((Match)testingData.DataPoints[i].Reference).HomeOdds + " vs " + ((Match)testingData.DataPoints[i].Reference).AwayOdds);
                Console.WriteLine("...");
                var tip = network.Run(testingData.Inputs()[i]);

                var success = SuccessConditionGoalAndPoints(tip, testingData.Outputs()[i]);
                var returns = BetReturnGoalAndPoints(tip, testingData.Outputs()[i], ((Match)testingData.DataPoints[i].Reference).HomeOdds, ((Match)testingData.DataPoints[i].Reference).AwayOdds);
                money -= 1.00;
                if (success)
                {
                    money += returns*1.00;
                }
                successes.Add(success);
            }
            Console.WriteLine("Success%: " + successes.Count(x => x) * 1.0 / trainingData.Outputs().Count * 1.0);
            Console.WriteLine("Money: " + money);
            Console.Read();
        }
        #endregion

        #region Optimizer
        private static void TestOptimizer()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            Console.WriteLine("Creating Optimizer...");
            var optimizer = new Optimizer();
            var output = "";

            optimizer.LowerLimitLayers = 1;
            optimizer.UpperLimitLayers = 2;
            optimizer.LowerLimitNeuronsInLayer = 3;
            optimizer.UpperLimitNeuronsInLayer = 3;
            //optimizer.LowerLimitEpochs = 50;
            //optimizer.LowerLimitEpochs = 50;

            Console.WriteLine("2014");

            Console.WriteLine("Creating training data...");
            var trainingData = tipper.LearnFromTo(2009, 0, 2014, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.LearnFromTo(2015, 0, 2015, 24);

            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints, Deconvert);

            Console.WriteLine(output);

            Console.Read();
        }
        #endregion

        #region TestFunction
        private static void TestFunction()
        {
            var tipper = new Tipper();
            var successes = 0;

            //var testingData1 = tipper.LearnFromTo(2008, 0, 2010, 24);
            var testingData1 = tipper.LearnFromTo(2009, 0, 2014, 24);

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

        #region Testing
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

            trainingData1 = tipper.LearnFromTo(2010, 0, 2014, 24);
            testingData1 = tipper.LearnFromTo(2015, 1, 2015, 24);

            trainingData2 = tipper.LearnFromTo(2013, 1, 2013, 24);
            testingData2 = tipper.LearnFromTo(2014, 1, 2014, 24);

            //Build
            Console.WriteLine("First...");
            Network network = new Network(trainingData1.DataPoints[0].Inputs.Count, new List<int>() { 3, 4 }, trainingData1.DataPoints[0].Outputs.Count);
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
        #endregion

        #region Helpers
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

        public static double BetReturnGoalAndPoints(List<double> predicted, List<double> actual, double homeOdds, double awayOdds)
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
                return homeOdds;
            if (phScore < paScore && ahScore < aaScore)
                return awayOdds;
            if (phScore == paScore && ahScore == aaScore)
                return 1.00;
            return 0.00;
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
        #endregion
    }
}
