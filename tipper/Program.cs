using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using AForge.Neuro;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using GeneticAlgorithm;
using GeneticAlgorithm.TraitTypes;
using GeneticAlgorithm.V2;
using GeneticArtificialNeuralNetwork;
using Tipper.Betting;
using Utilities;
using Network = ArtificialNeuralNetwork.Network;

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
                    case ("C"):
                        TestComplete();
                        break;
                    //case ("F"):
                    //    TestFunction();
                    //    break;
                    case ("E"):
                        TipEarlyRounds();
                        break;
                    case ("F"):
                        TipFullSeason();
                        break;
                    case ("GT"):
                        GeneticAlgorithmTest();
                        break;
                    case ("G"):
                        GeneticTestComplete();
                        break;
                    case ("O"):
                        TestOptimizer();
                        break;
                    case ("P"):
                        PrintActualResults();
                        break;
                    case ("Q"):
                        loop = false;
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
            Console.WriteLine("Tip [F]ull season");
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
            const int idealPopulation = 1250;
            const int matchesInSeason = 196;
            const int maxLoops = 500;

            //Load inputs for full full Dataset
            Console.WriteLine("Creating Tipper, Dataset etc.");
            var guid = Guid.NewGuid().ToString();
            var tipper = new Tipper();
            List<int> numGames = tipper.League.Seasons.Select(s => s.GetMatches().Count).ToList();
            var data = tipper.BuildFullDataSet();

            //Total inputs: 8 * 5 * 6 = 240
            Console.WriteLine("Creating list of actors...");

            //Create first population
            var actors = new List<BettingActor>();
            var datatrain = new Data();
            var datatest = new Data();
            datatrain.SuccessCondition = SuccessConditionTotalPrint;
            datatest.SuccessCondition = SuccessConditionTotalPrint;

            const int numScenarios = 3;
            var modulo = 0 % numScenarios;
            //0 = (start of 2011 season, end of 2014 season)
            //1 = (start of 2010 season, end of 2013 season)
            //2 = (start of 2009 season, end of 2012 season)
            //3 = (start of 2008 season, end of 2011 season)

            var start = 0;
            var size = 0;
            var completedGames = numGames.Count - 1;
            for (var i = 1; i < completedGames; i++)
            {
                if (i < numScenarios - (modulo))
                {
                    start += numGames[i];
                }
                else if (i < (completedGames - (modulo + 1)))
                {
                    size += numGames[i];
                }
            }
            datatrain.DataPoints = data.DataPoints.GetRange(start, size);
            datatest.DataPoints = data.DataPoints.GetRange(start + size, numGames[numGames.Count - (modulo + 1)]);

            //Train networks
            var networkActor = NetworkActor.BestGuessNetworkActor();
            networkActor.Network = Network.Load("Network/91f5a8ef-ea1d-4384-b13a-ccc3df89970e.ann");
            networkActor.Facade.SetMask(new List<bool>
                {
                    false,false,false,true,true,true,true,true,false,false,false,true,true,false,false,true,true,false,true,true,true,false,false,false,true,false,true,false,false,false
                });
            actors = new List<BettingActor>();
            //networkActor.Train(datatrain);

            Console.WriteLine("Starting generational loop...");
            //Repeat a bunch of times
            var loop = 0;
            while (loop < maxLoops)
            {
                loop++;
                Console.WriteLine("Stating new generation " + loop + "...");
                Repopulate(actors, idealPopulation, loop, networkActor);

                switch (loop % 5)
                {
                    case (0):
                        //Train networks
                        networkActor = NetworkActor.BestGuessNetworkActor();
                        //networkActor.Network = Network.Load("Network/ee74807b-4ab7-4931-9701-492e5d33cf76.ann");//Earliest season
                        networkActor.Network = Network.Load("Network/83fe994e-b33d-4e5d-af83-4256df284b4e.ann");//2015
                        networkActor.Facade.SetMask(new List<bool>
                        {
                            //false,true,false,false,false,false,true,false,true,false,true,false,false,true,false,true,false,false,false,true,false,true,false,false,false,true,false,true,false,true
                            false,false,true,true,false,false,true,true,false,false,false,true,true,false,false,false,true,true,false,false,true,true,true,true,true,false,true,false,false,true
                        });
                        break; 
                    case (1):
                        //Train networks
                        networkActor = NetworkActor.BestGuessNetworkActor();
                        //networkActor.Network = Network.Load("Network/91f5a8ef-ea1d-4384-b13a-ccc3df89970e.ann");
                        networkActor.Network = Network.Load("Network/98128506-754b-4040-95de-3f82c5fb19d1.ann");
                        networkActor.Facade.SetMask(new List<bool>
                        {
                            //false,false,false,true,true,true,true,true,false,false,false,true,true,false,false,true,true,false,true,true,true,false,false,false,true,false,true,false,false,false
                            false,false,true,false,false,false,false,false,true,true,false,false,false,false,false,true,true,true,false,false,false,false,false,true,true,true,false,false,false,true
                        });
                        break; 
                    case (2):
                        //Train networks
                        networkActor = NetworkActor.BestGuessNetworkActor();
                        //networkActor.Network = Network.Load("Network/738d0d29-8190-4ca9-9554-cff9dcf1dd45.ann");
                        networkActor.Network = Network.Load("Network/282c87b1-3042-4151-a460-fa6c9bda7793.ann");
                        networkActor.Facade.SetMask(new List<bool>
                        {
                            //false,false,false,false,false,false,false,false,true,true,true,false,true,true,false,true,false,false,false,false,true,false,false,true,false,true,false,false,false,true
                            false,false,true,false,true,false,false,true,true,false,false,true,false,false,false,true,true,true,false,false,false,true,false,true,false,true,false,false,false,false
                        });
                        break; 
                    case (3):
                        //Train networks
                        networkActor = NetworkActor.BestGuessNetworkActor();
                        //networkActor.Network = Network.Load("Network/90fa1692-d99e-4b74-bf17-e5adc1f1786e.ann");
                        networkActor.Network = Network.Load("Network/45939534-73a8-4dcc-9df7-2d916e402a06.ann");
                        networkActor.Facade.SetMask(new List<bool>
                        {
                            //false,false,false,false,false,false,true,true,false,true,false,false,true,false,false,true,true,false,true,false,true,true,true,false,false,false,false,false,true,true
                            false,false,true,true,false,true,false,true,false,false,true,true,false,false,false,true,false,true,false,false,false,false,false,true,true,false,false,false,false,true
                        });
                        break;   
                    case (4):
                        //Train networks
                        networkActor = NetworkActor.BestGuessNetworkActor();
                        //networkActor.Network = Network.Load("Network/39ce5003-24fd-4789-a2be-354989290935.ann");
                        networkActor.Network = Network.Load("Network/0f99924a-b840-49d5-8212-b23cf74660bf.ann");
                        networkActor.Facade.SetMask(new List<bool>
                        {
                            //false,false,false,false,true,false,true,true,true,false,false,true,false,false,false,false,true,false,true,false,true,false,true,true,true,true,false,false,false,false
                            false,true,false,false,true,false,false,false,true,true,true,true,true,false,true,false,true,false,true,true,false,true,true,false,false,true,true,false,true,true
                        });
                        break;   
                }

                foreach (var actor in actors)
                {
                    //Test population
                    actor.Test(datatest);
                    //Console.WriteLine("Actor has a success rate of {0}%", actor.GetFitness());
                    
                }
                //Save all

                //Sort Best to worst
                actors.Sort((x, y) => y.GetFitness().CompareTo(x.GetFitness()));
                //SaveActors(actors, guid, loop);
                SaveLog(actors, guid, loop);
                //cull the weakest
                actors.RemoveRange(actors.Count / 3, (actors.Count * 2 / 3));

                foreach (var actor in actors.Take(10))
                {
                    Console.WriteLine("Actor {0} has a season profit of ${1:0.00}", actor.Name, actor.GetFitness());

                }
            }
            Console.WriteLine("Actor {0} has a season profit of ${1:0.00}", 0, actors[0].GetFitness());

            Console.ReadLine();
        }

        public bool BettingRuleSimple(double margin)
        {
            return margin > 10;
        }

        public static List<BettingActor> Repopulate(List<BettingActor> actors, int targetPopulation, int generation, NetworkActor networkActor)
        {
            const int minPopulation = 50;
            const int numOutputs = 4;
            var count = actors.Count;
            var random = new Random(DateTime.Now.Millisecond + DateTime.Now.Second + DateTime.Now.Minute);

            if (count == 0)
            {
                var bettingActor = BettingActor.GetBestGuessBettingActor();
                bettingActor.NetworkActor = networkActor;
                bettingActor.Rules = new List<BettingRule>()
                {
                    new BettingRule()
                    {
                        Priority = 1,
                        Wager = 1,
                        Threshold = 18
                    },
                    new BettingRule()
                    {
                        Priority = 1,
                        Wager = 10,
                        Threshold = 36
                    }
                };
                actors.Add(bettingActor);
            }
            if (count < minPopulation)
            {
                for (var i = count; i < targetPopulation; i++)
                {
                    var bettingActor = BettingActorGenes.GenerateRandomActor(random);
                    bettingActor.NetworkActor = networkActor;
                    actors.Add(bettingActor);
                }
            }
            else
            {
                var actorGenes = BettingActorGenes.GenerateRepresentative(actors, random);
                for (var i = count; i < targetPopulation; i++)
                {
                    var bettingActor = actorGenes.GenerateBettingActor(numOutputs);
                    bettingActor.NetworkActor = networkActor;
                    actors.Add(bettingActor);
                }
            }

            foreach (var actor in actors)
            {
                actor.Generations.Add(generation);
                actor.Money = 0;
                actor.NetworkActor = networkActor;
                //actor.NetworkActor.RefreshNetwork();
            }

            return actors;
        }

        private static void SaveActors(List<BettingActor> actors, string guid, int loop)
        {
            var str = "";
            foreach (var actor in actors)
            {
                Network.Save(actor.NetworkActor.Network);
            }
            foreach (var actor in actors)
            {
                str += "<actor>\n";
                str += actor.NetworkActor.Stringify() + "\n";
                str += "</actor>\n";
            }

            Filey.Save(str, "GeneticArtificialNeuralNetwork/betting/" + guid + "-" + loop + ".gannb");
        }

        private static void SaveLog(List<BettingActor> actors, string guid, int loop)
        {
            var filename = "GeneticArtificialNeuralNetworkLog/betting/" + guid + "-" + loop + ".gannb";
            var str = Filey.Load(filename);
            str += "===============" + "\n";
            str += "Generation:    " + loop + "\n";
            str += "===============" + "\n";
            foreach (var actor in actors)
            {
                var ruleStr = "";
                str += "Actor:         " + actor.Name + "\n";
                str += "Money:         " + actor.GetFitness() + "\n";
                str += "Rules:         ";
                ruleStr += "{" +
                           actor.Rules.OrderBy(a => a.Priority)
                               .Aggregate(ruleStr,
                                   (current, rule) =>
                                       current + ("(M > " + rule.Threshold + " ? " + rule.Wager + ": 0),"))
                               .TrimEnd(',') + "}" + "\n";
                str += ruleStr;
                str += "Generations:   " + string.Join(",", actor.Generations.Select(x => x.ToString()).ToArray()) + "\n";
                //str += "Time to train: " + actor.NetworkActor.TimeToTrain + "\n";
                //str += "Time to test:  " + actor.TimeToTest + "\n";
                str += "...\n";
            }

            Filey.Save(str, filename);
        }
        #endregion

        #region Genetic Algorithm
        private static void GeneticAlgorithmTest()
        {
            Console.WriteLine("Setting up genetic algorithm environment");
            var genes = new List<GeneticAlgorithm.V2.GeneDefinition>()
            {
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=5},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=5},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=10},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=5},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=5},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=10}
            };

            var experiment = new GeneticAlgorithm.V2.Experiment()
            {
                GeneDefinitions = genes,
                EvaluationCallbacks = new List<Func<List<double>, double>>(){ EvaluationCallback},
                Generations = 50,
                CorePopulation = 10
            };
            experiment.Run();

            var result = experiment.Test(EvaluationMethod);
            Console.WriteLine("Result: {0:N2}", result);
            Console.ReadLine();
        }

        public static double EvaluationCallback(List<double> values)
        {
            const int target = 250;
            var result = EvaluationMethod(values);
            var err = Math.Abs(target-result);
            return err;
        }

        public static double EvaluationMethod(List<double> values)
        {
            if (values.Count != 6)
                Console.WriteLine("Wrong number of genes in callback {0}", values.Count);

            var a = values[0];
            var b = values[1];
            var c = values[2];
            var d = values[3];
            var e = values[4];
            var f = values[5];
            var result = (a * b) * (10 - c) + (d * e) * (10 - f);
            return result;
        }

        #endregion

        #region Genetic Test Complete

        private static void GeneticTestComplete()
        {
            Console.WriteLine("Setting up genetic algorithm environment");
            var year = 2016;
            var filename = @"TestResultsGA\test_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
            var genes = new List<GeneticAlgorithm.V2.GeneDefinition>()
            {
                new GeneticAlgorithm.V2.GeneDefinition(){Min=1,Max=2, Reference = "Layers"},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=1,Max=9, Reference = "Neurons"},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=50,Max=500, Reference = "Epochs"},//500
                new GeneticAlgorithm.V2.GeneDefinition(){Min=9,Max=9, Reference = "Training"},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=year,Max=year, Reference = "Testing"},
                new GeneticAlgorithm.V2.GeneDefinition(){Min=0,Max=0, Reference = "Normalisation"},
                //TODO: Upgrade these to allow 2D
                //Scores
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ScoreTeam" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ScoreGround" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ScoreState" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ScoreDay" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ScoreShared" },
                //Shots
                
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ShotTeam" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ShotGround" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ShotState" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ShotDay" },
                new GeneticAlgorithm.V2.GeneDefinition() { Min = 1, Max = 37, Reference = "ShotShared" },
            };

            Console.WriteLine("Layers|Neurons|Epochs|Training|Testing|Years|Normalisation|Shared|Day|State|Ground|Team|Network|Time|Accuracy");

            var experiment = new GeneticAlgorithm.V2.Experiment()
            {
                GeneDefinitions = genes,
                EvaluationCallbacks = new List<Func<List<double>, double>> { MultiDimensionalCallbackTester2015, MultiDimensionalCallbackTester2016},
                Generations = 30,
                CorePopulation = 10
            };

            experiment.Run();
            Console.WriteLine("Done!");
            //var result = experiment.Test(EvaluationMethod);
            //Console.WriteLine("Result: {0:N2}", result);
        
            //var output = multiDimensionalTester.Run();
            //Filey.Append(output, filename);
            Console.ReadLine();
        }

        public static double MultiDimensionalCallbackTester2015(List<double> values)
        {
            return MultiDimensionalCallbackTester(values, 2015);
        }

        public static double MultiDimensionalCallbackTester2016(List<double> values)
        {
            return MultiDimensionalCallbackTester(values, 2016);
        }

        public static double MultiDimensionalCallbackTester(List<double> values, int year)
        {
            var roundedValues = values.Select(Convert.ToInt32).ToList();
            var numberOfHiddenLayers = (int)roundedValues[0];
            var numberOfHiddenNeuronsPerLayer = (int)roundedValues[1];
            var maximumNumberOfEpochs = (int)roundedValues[2];
            var training = year - (int)roundedValues[3];
            //TODO: don't be lazy, move this, don't ignore it
            var testing = year;//(int)roundedValues[4];
            var normalisation
                = (int)roundedValues[5] == 0 ? Numbery.NormalisationMethod.Normal
                : (int)roundedValues[5] == 1 ? Numbery.NormalisationMethod.Gradiated
                : (int)roundedValues[5] == 2 ? Numbery.NormalisationMethod.Asymptotic
                : (int)roundedValues[5] == 3 ? Numbery.NormalisationMethod.AsymptoticSharp
                : (int)roundedValues[5] == 4 ? Numbery.NormalisationMethod.AsymptoticExtraSharp
                : Numbery.NormalisationMethod.AsymptoticSmooth;

            var interpretationTeamScore= new List<int>(){ roundedValues[6]};
            var interpretationGroundScore= new List<int>(){roundedValues[7]};
            var interpretationStateScore= new List<int>(){roundedValues[8]};
            var interpretationDayScore= new List<int>(){roundedValues[9]};
            var interpretationSharedScore= new List<int>(){roundedValues[10]};

            var interpretationTeamShots = new List<int>() { roundedValues[11] };
            var interpretationGroundShots = new List<int>() { roundedValues[12] };
            var interpretationStateShots = new List<int>() { roundedValues[13] };
            var interpretationDayShots = new List<int>() { roundedValues[14] };
            var interpretationSharedShots = new List<int>() { roundedValues[15] };
            
            List<List<int>> interpretation = new List<List<int>>
            {
                interpretationTeamScore, interpretationGroundScore, interpretationStateScore, interpretationDayScore, interpretationSharedScore, 
                interpretationTeamShots, interpretationGroundShots, interpretationStateShots, interpretationDayShots, interpretationSharedShots
            };
            var tipper = new Tipper();
            tipper.NormalisationMethod = normalisation;

            var stopWatch = new Stopwatch();
            var trainingStart = training;
            var trainingEnd = testing - 1;

            var roundStart = 0;
            var roundEnd = 23;


            var trainingData = tipper.GetMatchDataBetween(trainingStart, roundStart, trainingEnd + 1, 0, interpretation);
            var testingData = tipper.GetMatchDataBetween(testing, roundStart, testing, roundEnd, interpretation);
            trainingData.SuccessCondition = SuccessConditionTotalPrint;
            testingData.SuccessCondition = SuccessConditionTotalPrint;
            //Console.WriteLine(trainingData.Inputs()[0].Select(i => i.ToString()).Aggregate((i, j) => i.ToString() + "," + j.ToString())); //this was for your art project, feel free to delete
            //Console.WriteLine(trainingData.Outputs()[0].Select(i => i.ToString()).Aggregate((i, j) => i.ToString() + "," + j.ToString()));
            //Console.WriteLine(trainingData.Inputs().Select(i => i.Max()).Max());
            //Console.WriteLine(trainingData.Inputs().Select(i => i.Min()).Min());
            var fullinputs = trainingData.Inputs().SelectMany(i => i).ToList();
            Filey.Append(fullinputs.Select(i => String.Format("{0:N2}", i)).Aggregate((i, j) => i + "," + j), @"TestResults\temp_inputs_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            stopWatch.Start();
            tipper.Net = Network.CreateNetwork(trainingData, numberOfHiddenLayers, numberOfHiddenNeuronsPerLayer,
                TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = maximumNumberOfEpochs;
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());
            stopWatch.Stop();

            var successes =
                testingData.Inputs()
                    .Select(t => tipper.Net.Run(t))
                    .Where(
                        (result, i) =>
                            testingData.SuccessCondition(result, testingData.DataPoints[i].Outputs, null))
                    .Count();
            var successRate = 100 * (double)successes / testingData.DataPoints.Count;

            var index = 0;
            foreach (var m in testingData.Inputs())
            {
                var result = tipper.Net.Run(m);
                testingData.SuccessCondition(result, testingData.DataPoints[index].Outputs,
                    @"Tips\test_" +
                    DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) +
                    "_" + tipper.Net.Id + "_tips.txt");
                index++;
            }

            var time = ((double)stopWatch.ElapsedMilliseconds / 1000);
            /*var output = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13:N2}|{14:N2}%", (int)roundedValues[0],
                (int)roundedValues[1], (int)roundedValues[2], (int)roundedValues[3], year, (testing - training),
                (int)roundedValues[5] == 0 ? "Normal" : (int)roundedValues[5] == 1 ? "Gradiated" : (int)roundedValues[5] == 2 ? "Asymptotic" : (int)roundedValues[5] == 3 ? "AsymptoticSharp" : "AsymptoticSmooth",
                string.Join(",", interpretationSharedScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationDayScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationStateScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationGroundScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationTeamScore.Select(n => n.ToString()).ToArray()),
                tipper.Net.Id, time,
                successRate);*/
            var output = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}|{18:N2}|{19:N2}%", (int)roundedValues[0],
                (int)roundedValues[1], (int)roundedValues[2], (int)roundedValues[3], year, (testing - training),
                (int)roundedValues[5] == 0 ? "Normal" : (int)roundedValues[5] == 1 ? "Gradiated" : (int)roundedValues[5] == 2 ? "Asymptotic" : (int)roundedValues[5] == 3 ? "AsymptoticSharp" : "AsymptoticSmooth",
                string.Join(",", interpretationSharedScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationDayScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationStateScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationGroundScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationTeamScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationSharedShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationDayShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationStateShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationGroundShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationTeamShots.Select(n => n.ToString()).ToArray()),
                tipper.Net.Id, time,
                successRate);
            Console.WriteLine(output);
            return 100 - successRate;
        }
        #endregion

        #region Test Complete

        private static void TestComplete()
        {
                MetaTest(0);
        }


        public static void MetaTest(int index)
        {
            Console.WriteLine("Layers|Neurons|Epochs|Training|Testing|Years|Normalisation|Shared|Day|State|Ground|Team|Network|Time|Accuracy");
            var year = 2016;
            var filename = @"TestResults\test_" + DateTime.Now.ToString("yyyyMMddHHmmss") +".txt";

            var multiDimensionalTester = new MultiDimensionalTester.MultiDimensionalTester();
            //Num Layers
            multiDimensionalTester.AddParameterGroup(new List<int> { 1 });
            //Neurons in
            multiDimensionalTester.AddParameterGroup(new List<int> { 3 });
            //Max epochs
            multiDimensionalTester.AddParameterGroup(new List<int> { 500 });
            //Training season
            multiDimensionalTester.AddParameterGroup(new List<int> { year-9});
            //Testing Season
            multiDimensionalTester.AddParameterGroup(new List<int> { year });
            //Normalisation
            multiDimensionalTester.AddParameterGroup(new List<int> { 4});

            for (int i = 0; i < 10; i++)
            {
                multiDimensionalTester.AddParameterGroup(new List<int> { 0, 2});
            }

            multiDimensionalTester.Callback = MultiDimensionalCallbackTester;
            var output = multiDimensionalTester.Run();
            Filey.Append(output, filename);
        }

        public static string MultiDimensionalCallbackTester(object[] args)
        {
            var numberOfHiddenLayers = (int)args[0];
            var numberOfHiddenNeuronsPerLayer = (int)args[1];
            var maximumNumberOfEpochs = (int)args[2];
            var training = (int)args[3];
            var testing = (int)args[4];
            var normalisation 
                = (int)args[5] == 0 ? Numbery.NormalisationMethod.Normal
                : (int)args[5] == 1 ? Numbery.NormalisationMethod.Gradiated
                : (int)args[5] == 2 ? Numbery.NormalisationMethod.Asymptotic 
                : (int)args[5] == 3 ? Numbery.NormalisationMethod.AsymptoticSharp 
                : (int)args[5] == 4 ? Numbery.NormalisationMethod.AsymptoticExtraSharp
                : Numbery.NormalisationMethod.AsymptoticSmooth;

            var interpretationTeamScore 
                = (int)args[6] == 0 ? new List<int> {1, 3, 5}
                : (int)args[6] == 1 ? new List<int> {9, 13, 17} 
                : (int)args[6] == 2 ? new List<int> {25, 31, 37}
                : new List<int> {1, 3, 5, 9, 13, 17, 25, 31, 37};
            var interpretationGroundScore 
                = (int)args[7] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[7] == 1 ? new List<int> { 9, 13, 17 } 
                : (int)args[7] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationStateScore 
                = (int)args[8] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[8] == 1 ? new List<int> { 9, 13, 17 } 
                : (int)args[8] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationDayScore 
                = (int)args[9] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[9] == 1 ? new List<int> { 9, 13, 17 } 
                : (int)args[9] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationSharedScore 
                = (int)args[10] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[10] == 1 ? new List<int> { 9, 13, 17 } 
                : (int)args[10] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            /*
            var interpretationTeamShots
                = (int)args[10] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[10] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[10] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationGroundShots
                = (int)args[11] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[11] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[11] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationStateShots
                = (int)args[12] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[12] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[12] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationDayShots
                = (int)args[13] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[13] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[13] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationSharedShots
                = (int)args[14] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[14] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[14] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            */
            List<List<int>> interpretation = new List<List<int>>
            {
                interpretationTeamScore, interpretationGroundScore, interpretationStateScore, interpretationDayScore, interpretationSharedScore, 
                //interpretationTeamShots, interpretationGroundShots, interpretationStateShots, interpretationDayShots, interpretationSharedShots
            };
            var tipper = new Tipper();
            tipper.NormalisationMethod = normalisation;

            var stopWatch = new Stopwatch();
            var trainingStart = training;
            var trainingEnd = testing - 1;

            var roundStart = 0;
            var roundEnd = 23;


            var trainingData = tipper.GetMatchDataBetween(trainingStart, roundStart, trainingEnd + 1, 0, interpretation);
            var testingData = tipper.GetMatchDataBetween(testing, roundStart, testing, roundEnd, interpretation);
            trainingData.SuccessCondition = SuccessConditionTotalPrint;
            testingData.SuccessCondition = SuccessConditionTotalPrint;
            //Console.WriteLine(trainingData.Inputs()[0].Select(i => i.ToString()).Aggregate((i, j) => i.ToString() + "," + j.ToString())); //this was for your art project, feel free to delete
            //Console.WriteLine(trainingData.Outputs()[0].Select(i => i.ToString()).Aggregate((i, j) => i.ToString() + "," + j.ToString()));
            //Console.WriteLine(trainingData.Inputs().Select(i => i.Max()).Max());
            //Console.WriteLine(trainingData.Inputs().Select(i => i.Min()).Min());
            var fullinputs = trainingData.Inputs().SelectMany(i => i).ToList();
            Filey.Append(fullinputs.Select(i => String.Format("{0:N2}", i)).Aggregate((i, j) => i + "," + j), @"TestResults\temp_inputs_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt");
            stopWatch.Start();
            tipper.Net = Network.CreateNetwork(trainingData, numberOfHiddenLayers, numberOfHiddenNeuronsPerLayer,
                TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = maximumNumberOfEpochs;
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());
            stopWatch.Stop();

            var successes =
                testingData.Inputs()
                    .Select(t => tipper.Net.Run(t))
                    .Where(
                        (result, i) =>
                            testingData.SuccessCondition(result, testingData.DataPoints[i].Outputs, null))
                    .Count();
            var successRate = 100*(double) successes/testingData.DataPoints.Count;

            var index = 0;
            foreach (var m in testingData.Inputs())
            {
                var result = tipper.Net.Run(m);
                testingData.SuccessCondition(result, testingData.DataPoints[index].Outputs,
                    @"Tips\test_" +
                    DateTime.Now.ToString("yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture) +
                    "_" + tipper.Net.Id + "_tips.txt");
                index++;
            }

            var time = ((double) stopWatch.ElapsedMilliseconds/1000);
            var output = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13:N2}|{14:N2}%", (int) args[0],
                (int) args[1], (int) args[2], (int) args[3], (int) args[4], (testing - training),
                (int)args[5] == 0 ? "Normal" : (int)args[5] == 1 ? "Gradiated" : (int)args[5] == 2 ? "Asymptotic" : (int)args[5] == 3 ? "AsymptoticSharp" : "AsymptoticSmooth",
                string.Join(",", interpretationSharedScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationDayScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationStateScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationGroundScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationTeamScore.Select(n => n.ToString()).ToArray()),
                tipper.Net.Id, time,
                successRate);
            /*var output = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17:N2}|{18:N2}%", (int)args[0],
                (int)args[1], (int)args[2], (int)args[3], (int)args[4], (testing - training),
                string.Join(",", interpretationSharedScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationDayScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationStateScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationGroundScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationTeamScore.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationSharedShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationDayShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationStateShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationGroundShots.Select(n => n.ToString()).ToArray()),
                string.Join(",", interpretationTeamShots.Select(n => n.ToString()).ToArray()),
                tipper.Net.Id, time,
                successRate);*/
            Console.WriteLine(output);
            return output;
        }
        #endregion

        #region Tip early rounds
        private static void TipEarlyRounds()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();

            Console.WriteLine("Init Neural Network...");

            //Years	Shared	    Day	        State	Ground	    Team
            //6	    25,31,37	25,31,37	1,3,5	9,13,17	    9,13,17

            var interpretationTeam = new List<int> { 25, 31, 37 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 9, 13, 17 };
            var interpretationShared = new List<int> { 9, 13, 17 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            var trainingData = tipper.GetMatchDataBetween(2010, 0, 2016, 0, interpretation);
            trainingData.SuccessCondition = SuccessConditionTotalPrint;

            //6b376a71-5cb0-41ca-95fb-02daf1db536f
            Console.WriteLine("Create network...");
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            Console.WriteLine("Network: " + tipper.Net.Id);
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

            Console.WriteLine("Tip 2016...");
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == 2016).SelectMany(s => s.Rounds).ToList())
            {
                Console.WriteLine("Tip Round {0} ...", r.Number);
                tipper.PredictWinner(r.Year, r.Number, true, interpretation);
            }
            Console.ReadLine();
        }
        #endregion

        #region Tip full season
        private static void TipFullSeason()
        {
            Console.WriteLine("Loading data...");
            var tipper = new Tipper();
            var year = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year;
            var round = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Rounds.Where(r => r.Matches.All(m => m.TotalScore() > 0)).OrderByDescending(r => r.Number).First().Number;
            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Last completed year:" + year);
            Console.WriteLine("Last completed round:" + round);

            
            //Based on Test scenario #670
            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            var trainingData = tipper.GetMatchDataBetween(year - 9, 0, year, round, interpretation);
            trainingData.SuccessCondition = SuccessConditionTotalPrint;

            //6b376a71-5cb0-41ca-95fb-02daf1db536f
            
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 1000;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

            Console.WriteLine("Tip 2017...");
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).ToList())
            {
                Console.WriteLine("Tip Round {0} ...", r.Number);
                tipper.PredictWinner(r.Year, r.Number, true, interpretation);
            }
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
            var trainingData = tipper.GetMatchDataBetween(2009, 0, 2014, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.GetMatchDataBetween(2015, 0, 2015, 24);

            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(trainingData, testingData, SuccessConditionGoalAndPoints, Deconvert);

            Console.WriteLine(output);

            Console.Read();
        }
        #endregion
        
        #region PrintActualResults
        private static void PrintActualResults()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            var season = tipper.League.Seasons.First(s => s.Year == 2016);
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == 2016).SelectMany(s => s.Rounds).ToList())
            {
                Console.WriteLine("Results Round {0} ...", r.Number);
                tipper.StateMatchesult(r.Year, r.Number);
            }
            Console.ReadLine();
        }
        #endregion

        #region TestFunction

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
            Console.WriteLine("Loading data...");
            var tipper = new Tipper();
            var year = 2017;
            var round = 0;

            //Based on Test scenario #670
            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            var trainingData = tipper.GetMatchDataBetween(year - 9, 0, year, round, interpretation);
            trainingData.SuccessCondition = SuccessConditionTotalPrint;

            //6b376a71-5cb0-41ca-95fb-02daf1db536f

            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 1000;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

            Console.WriteLine("Tip 2017...");
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).ToList())
            {
                Console.WriteLine("Tip Round {0} ...", r.Number);
                tipper.PredictWinner(r.Year, r.Number, true, interpretation);
            }
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

        public static bool SuccessConditionGoalAndPointsPrint(List<double> predicted, List<double> actual, string print)
        {
            Func<double, double> rule = (m => m > 27.0 ? 15.00 : 0.00);

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

            var margin = Math.Abs(phScore - ahScore);
            var wager = rule(margin);

            if(print == "")
                Console.WriteLine("[{0}, {1} Vs {2}, {3}] Suggested Bet: ${4:0.00}", phScore, paScore, ahScore, aaScore, wager);
            else if (!string.IsNullOrEmpty(print))
                Filey.Save(string.Format("\n[{0}, {1} Vs {2}, {3}] Suggested Bet: ${4:0.00}", phScore, paScore, ahScore, aaScore, wager), print);

            if (phScore > paScore && ahScore > aaScore)
                return true;
            if (phScore < paScore && ahScore < aaScore)
                return true;
            if (phScore == paScore && ahScore == aaScore)
                return true;
            return false;
        }

        public static bool SuccessConditionLadderPrint(List<double> predicted, List<double> actual, string print)
        {
            var phLadder = Numbery.Denormalise(predicted[0], Util.MaxLadderPoints);
            var paLadder = Numbery.Denormalise(predicted[1], Util.MaxLadderPoints);

            var ahLadder = Numbery.Denormalise(actual[0], Util.MaxLadderPoints);
            var aaLadder = Numbery.Denormalise(actual[1], Util.MaxLadderPoints);

            if (print == "")
                Console.WriteLine("[{0}, {1} Vs {2}, {3}] Suggested Bet: ${8:0.00}", phLadder, paLadder, ahLadder, aaLadder);
            else if (!string.IsNullOrEmpty(print))
                Filey.Save(string.Format("[{0}, {1} Vs {2}, {3}] Suggested Bet: ${8:0.00}", phLadder, paLadder, ahLadder, aaLadder), print);

            if (phLadder > paLadder && ahLadder > aaLadder)
                return true;
            if (phLadder < paLadder && ahLadder < aaLadder)
                return true;
            if (phLadder == paLadder && ahLadder == aaLadder)
                return true;
            return false;
        }

        public static bool SuccessConditionTotalPrint(List<double> predicted, List<double> actual, string print)
        {
            var phTotal = Numbery.Denormalise(predicted[0], Util.MaxScore);
            var paTotal = Numbery.Denormalise(predicted[1], Util.MaxScore);

            var ahTotal = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaTotal = Numbery.Denormalise(actual[1], Util.MaxScore);

            if (print == "")
                Console.WriteLine("[{0}, {1} Vs {2}, {3}]", phTotal, paTotal, ahTotal, aaTotal);
            else if (!string.IsNullOrEmpty(print))
                Filey.Append(string.Format("[{0}, {1} Vs {2}, {3}]", phTotal, paTotal, ahTotal, aaTotal), print);

            if (phTotal > paTotal && ahTotal > aaTotal)
                return true;
            if (phTotal < paTotal && ahTotal < aaTotal)
                return true;
            if (phTotal == paTotal && ahTotal == aaTotal)
                return true;
            return false;
        }

        public static bool SuccessConditionTotalAsymptotic(List<double> predicted, List<double> actual, string legacy)
        {
            var phTotal = Numbery.Denormalise(predicted[0], Util.MaxScore, Numbery.NormalisationMethod.Asymptotic);
            var paTotal = Numbery.Denormalise(predicted[1], Util.MaxScore, Numbery.NormalisationMethod.Asymptotic);

            var ahTotal = Numbery.Denormalise(actual[0], Util.MaxScore);
            var aaTotal = Numbery.Denormalise(actual[1], Util.MaxScore);


            if (phTotal > paTotal && ahTotal > aaTotal)
                return true;
            if (phTotal < paTotal && ahTotal < aaTotal)
                return true;
            if (phTotal == paTotal && ahTotal == aaTotal)
                return true;
            return false;
        }

        public static bool SuccessConditionLadderGoalsAndPointsPrint(List<double> predicted, List<double> actual, string print)
        {
            Func<double, double> rule = (m => m > 27.0 ? 15.00 : 0.00);

            var phLadder = Numbery.Denormalise(predicted[0], Util.MaxLadderPoints);
            var paLadder = Numbery.Denormalise(predicted[1], Util.MaxLadderPoints);
            var phGoals = Numbery.Denormalise(predicted[2], Util.MaxGoals);
            var phPoints = Numbery.Denormalise(predicted[3], Util.MaxPoints);
            var paGoals = Numbery.Denormalise(predicted[4], Util.MaxGoals);
            var paPoints = Numbery.Denormalise(predicted[5], Util.MaxPoints);
            var phScore = phGoals * 6 +phPoints;
            var paScore = paGoals * 6 +paPoints;

            var ahLadder = Numbery.Denormalise(actual[0], Util.MaxLadderPoints);
            var aaLadder = Numbery.Denormalise(actual[1], Util.MaxLadderPoints);
            var ahGoals = Numbery.Denormalise(actual[2], Util.MaxGoals);
            var ahPoints = Numbery.Denormalise(actual[3], Util.MaxPoints);
            var aaGoals = Numbery.Denormalise(actual[4], Util.MaxGoals);
            var aaPoints = Numbery.Denormalise(actual[5], Util.MaxPoints);
            var ahScore = ahGoals * 6 +ahPoints;
            var aaScore = aaGoals * 6 +aaPoints;

            var margin = Math.Abs(phScore - ahScore);
            var wager = rule(margin);

            if (print == "")
                Console.WriteLine("[{0}: {1}, {2}: {3} Vs {4}: {5}, {6}: {7}] Suggested Bet: ${8:0.00}", phLadder, phScore, paLadder, paScore, ahLadder, ahScore, aaLadder, aaScore, wager);
            else if (!string.IsNullOrEmpty(print))
                Filey.Save(string.Format("[{0}: {1}, {2}: {3} Vs {4}: {5}, {6}: {7}] Suggested Bet: ${8:0.00}", phLadder, phScore, paLadder, paScore, ahLadder, ahScore, aaLadder, aaScore, wager), print);

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
