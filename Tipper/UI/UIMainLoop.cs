using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using GeneticArtificialNeuralNetwork;
using Tipper.Betting;
using Utilities;

namespace Tipper.UI
{
    public class UIMainLoop
    {
        #region UI

        public static void LoadMainLoop()
        {
            var loop = true;
            Console.WriteLine("Footy Tipper");
            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("C"):
                        CompareSetupOptions();
                        break;
                    case ("F"):
                        TipFullSeason();
                        break;
                    case ("G"):
                        GeneticAlgorithmTest();
                        break;
                    case ("N"):
                        TipNextRound();
                        break;
                    case ("O"):
                        UIOptimizerLoop.LoadOptimizerLoop();
                        break;
                    case ("P"):
                        PrintActualResults();
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
            Console.WriteLine("[C]ompare setup options, generate report");
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

        public static void SaveActors(List<BettingActor> actors, string guid, int loop)
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

        #endregion

        #region Genetic Algorithm
        private static void GeneticAlgorithmTest()
        {
            const int idealPopulation = 25;
            const int maxLoops = 4;

            //Load inputs for full full Dataset
            Console.WriteLine("Creating Tipper, Dataset etc.");
            var guid = Guid.NewGuid().ToString();
            var tipper = new Tipper();
            List<int> numGames = tipper.League.Seasons.Select(s => s.GetMatches().Count).ToList();
            var data = tipper.BuildFullDataSet();

            //                
            var datatrain = new Data();
            var datatest = new Data();
            datatrain.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;
            datatest.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;

            const int numScenarios = 3;
            const int modulo = 0 % numScenarios;
            //0 = (start of 2011 season, end of 2014 season)
            //1 = (start of 2010 season, end of 2013 season)
            //2 = (start of 2009 season, end of 2012 season)
            ////3 = (start of 2008 season, end of 2011 season)

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
                    if (actor.Network.Error <= 0.0001)
                    {
                        Console.Write(actor.Id + ", ");
                        //Train population
                        actor.Train(datatrain);
                    }
                    //Test population
                    actor.Test(datatest);
                }

                //Sort Best to worst
                actors.Sort((x, y) => y.GetFitness().CompareTo(x.GetFitness()));

                //Save all
                SaveActors(actors, guid, loop);
                SaveLog(actors, guid, loop);

                //cull the weakest
                actors.RemoveRange(actors.Count / 3, (actors.Count * 2 / 3));

                foreach (var actor in actors)
                {
                    Console.WriteLine("Actor has a success rate of {0}%", actor.GetFitness());
                }
            }
            Console.WriteLine("Actor {0} has a success rate of {1}%", 0, actors[0].GetFitness());
            Console.ReadLine();
        }

        public static List<NetworkActor> Repopulate(List<NetworkActor> actors, int targetPopulation, int generation)
        {
            const int minPopulation = 10;
            const int numOutputs = 2;
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
                    var actorGenes = NetworkActorGenes.GenerateRandomRepresentative(random);
                    actors.Add(actorGenes.GenerateActor(numOutputs));
                }
            }
            else
            {
                var actorGenes = NetworkActorGenes.GenerateRepresentative(actors, random);
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
            const int outputs = 2;
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
                str += "Network:       " + actor.Network.Id + "\n";
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

        #region Tip next round
        private static void TipNextRound()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            var date = DateTime.Now;

            Console.WriteLine("Init Neural Network...");
            var trainingData = tipper.GetMatchDataBetween(2011, 0, 2016, 23);
            var testingData = tipper.GetMatchDataBetween(2016, 0, 2018, 23);
            testingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;

            Console.WriteLine("Create network...");
            var example = Network.Load("Network/91728224-2685-4f6d-a0f9-fb23a0968389.ann");
            tipper.Net = Network.CreateNetwork(trainingData, example.HLayers.Count, example.HLayers[0].Count, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);

            Console.WriteLine("Test network 2015");
            var successes = testingData.Inputs().Select(t => tipper.Net.Run(t)).Where((result, i) => testingData.SuccessCondition(result, testingData.DataPoints[i].Outputs, null)).Count();
            var successRate = 100 * (double)successes / testingData.DataPoints.Count;
            Console.WriteLine("Success rate: {0:N2}", successRate);

            Console.WriteLine("Tip 2015 round...");
            var tips = tipper.PredictNext(date, true);
            Console.WriteLine("..." + tips[0].HomeScore());
            Console.Read();
        }
        #endregion

        #region Test Complete

        private static void CompareSetupOptions()
        {
            MetaTest(0);
        }


        public static void MetaTest(int index)
        {
            Console.WriteLine("Layers|Neurons|Epochs|Training|Testing|Years|Shared|Day|State|Ground|Team|Network|Time|Accuracy");
            const int year = 2016;
            var filename = @"TestResults\test_" + year + ".txt";

            var multiDimensionalTester = new MultiDimensionalTester.MultiDimensionalTester();
            //Num Layers
            multiDimensionalTester.AddParameterGroup(new List<int> { 1 });
            //Neurons in
            multiDimensionalTester.AddParameterGroup(new List<int> { 3 });
            //Max epochs
            multiDimensionalTester.AddParameterGroup(new List<int> { 500 });
            //Training season
            multiDimensionalTester.AddParameterGroup(new List<int> { year - 9 });
            //Testing Season
            multiDimensionalTester.AddParameterGroup(new List<int> { year });

            for (int i = 0; i < 10; i++)
            {
                multiDimensionalTester.AddParameterGroup(new List<int> { 0, 2 });
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

            var interpretationTeamScore
                = (int)args[5] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[5] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[5] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationGroundScore
                = (int)args[6] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[6] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[6] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationStateScore
                = (int)args[7] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[7] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[7] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationDayScore
                = (int)args[8] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[8] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[8] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };
            var interpretationSharedScore
                = (int)args[9] == 0 ? new List<int> { 1, 3, 5 }
                : (int)args[9] == 1 ? new List<int> { 9, 13, 17 }
                : (int)args[9] == 2 ? new List<int> { 25, 31, 37 }
                : new List<int> { 1, 3, 5, 9, 13, 17, 25, 31, 37 };

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

            List<List<int>> interpretation = new List<List<int>>
            {
                interpretationTeamScore, interpretationGroundScore, interpretationStateScore, interpretationDayScore, interpretationSharedScore, 
                interpretationTeamShots, interpretationGroundShots, interpretationStateShots, interpretationDayShots, interpretationSharedShots
            };
            var tipper = new Tipper();

            var stopWatch = new Stopwatch();
            var trainingStart = training;
            var trainingEnd = testing - 1;

            const int roundStart = 0;
            const int roundEnd = 23;


            var trainingData = tipper.GetMatchDataBetween(trainingStart, roundStart, trainingEnd + 1, 0, interpretation);
            var testingData = tipper.GetMatchDataBetween(testing, roundStart, testing, roundEnd, interpretation);
            trainingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;
            testingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;
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
                    DateTime.Now.ToString("yyyyMMdd", CultureInfo.InvariantCulture) +
                    "_" + tipper.Net.Id + "_tips.txt");
                index++;
            }

            var time = ((double)stopWatch.ElapsedMilliseconds / 1000);
            var output = String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17:N2}|{18:N2}%", (int)args[0],
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
                successRate);
            Console.WriteLine(output);
            return output;
        }


        #endregion

        #region Tip full season

        private static void TipFullSeason()
        {
            var loop = true;
            const string options = "Tip [F]ull season, [T]est the alorithm used for tipping, or [B]oth?";
            Console.WriteLine("Cool, I can tip the full season four you.");
            Console.WriteLine(options);

            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("B"):
                        TestFullSeason();
                        JustTipFullSeason();
                        break;
                    case ("T"):
                        TestFullSeason();
                        break;
                    case ("F"):
                        JustTipFullSeason();
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    case ("?"):
                        Console.WriteLine(options);
                        break;
                }
            }
        }

        private static void JustTipFullSeason()
        {
            Console.WriteLine("Loading data...");
            //Load tipper
            var tipper = new Tipper();

            //Load last completed 
            var year = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year;
            var completedRounds =
                tipper.League.Seasons.Where(s => s.Rounds.Any())
                    .OrderByDescending(s => s.Year)
                    .First()
                    .Rounds.Where(r => r.Matches.All(m => m.TotalScore() > 0))
                    .ToList();
            var round = !completedRounds.Any() ? 0 : completedRounds.OrderByDescending(r => r.Number).First().Number;


            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Last completed year:" + year);
            Console.WriteLine("Last completed round:" + round);
            //Tip
            var predictions = SetUpTipper(tipper, year, round);
            var rounds = predictions.Select(p => p.RoundNumber).Distinct();
            foreach (var r in rounds)
            {
                Console.WriteLine("Tip Round {0} ...", r);
                Console.WriteLine(tipper.ResultToString(predictions.Where(p => p.RoundNumber == r).ToList()));
            }
        }

        private static void TestFullSeason()
        {
            Console.WriteLine("Loading data...");
            //Load tipper
            var tipper = new Tipper();

            //Load last completed 
            var year = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year - 1;
            const int round = 0;

            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Tipping year:" + year);

            //Tip
            var predictions = SetUpTipper(tipper, year, round);
            var actuals = tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).SelectMany(r => r.Matches).ToList();
            var successes = new List<bool>();
            //compare
            foreach (var p in predictions)
            {
                var predictedScores = new List<double>() { p.HomeTotal, p.AwayTotal };
                var actual = actuals.First(a => a.Date == p.Date && a.Home.Equals(p.Home));
                var actualScores = new List<double>() { actual.HomeScore().Total(), actual.AwayScore().Total() };
                var success = false;
                if (predictedScores[0] > predictedScores[1] && actualScores[0] > actualScores[1])
                    success = true;
                if (predictedScores[0] < predictedScores[1] && actualScores[0] < actualScores[1])
                    success = true;
                if (Math.Abs(predictedScores[0] - predictedScores[1]) < 0.1 && Math.Abs(actualScores[0] - actualScores[1]) < 0.1)
                    success = true;
                successes.Add(success);
            }
            Console.WriteLine("Correct: {0}, Incorrect: {1}, Success rate: {2}", successes.Count(s => s), successes.Count(s => !s), (double)successes.Count(s => s) / (double)successes.Count);
        }

        private static List<PredictedMatch> SetUpTipper(Tipper tipper, int year, int round)//Names are getting stupid
        {
            //Based on Test scenario #670
            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            var trainingData = tipper.GetMatchDataBetween(year - 10, 0, year, round, interpretation);
            trainingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;

            //Create Network
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 750;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");
            //Console.WriteLine("If it seams fast that's because you forgot to undo the epochs for testing");

            //Train Network
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

            //Print results
            Console.WriteLine("Tip {0}...", year);
            var predictions = new List<PredictedMatch>();
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).Where(r => r.Number > round).ToList())
            {
                predictions.AddRange(tipper.PredictWinners(r.Year, r.Number, interpretation));
            }
            return predictions;
        }
        #endregion

        #region Tip
        //TODO: Allow user selection for which round to tip
        private static void TipSpecific()
        {
            Console.WriteLine("Loading data");
            var tipper = new Tipper();
            //Based on Test scenario #670
            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };
            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            var trainingData = tipper.GetMatchDataBetween(2008, 0, 2017, 0, interpretation);

            Console.WriteLine("Training network..");
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3,
                TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 1000;
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());



            Console.WriteLine("Tipping Season...");
            var tips = tipper.Predict(2017, 1, true);

            Console.Read();
        }
        #endregion

        #region TipBetSeason
        private static void TipBetSeason()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            var money = 200.00;

            Console.WriteLine("Init Neural Network...");
            Network network = new Network(88, new List<int> { 1 }, 4);
            Console.WriteLine("Creating training data...");
            var trainingData = tipper.GetMatchDataBetween(2009, 0, 2011, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.GetMatchDataBetween(2015, 0, 2015, 24);

            Console.WriteLine("Training Neural Network...");
            network.Train(trainingData.Inputs(), trainingData.Outputs());

            Console.WriteLine("Testing Neural Network...");
            var successes = new List<bool>();
            for (var i = 0; i < testingData.Outputs().Count; i++)
            {
                var round = (i / 8) - 1;
                Console.WriteLine("Match: " + ((Match)testingData.DataPoints[i].Reference).Home.ApiName + " vs " + ((Match)testingData.DataPoints[i].Reference).Away.ApiName);
                Console.WriteLine("Odds: " + ((Match)testingData.DataPoints[i].Reference).HomeOdds + " vs " + ((Match)testingData.DataPoints[i].Reference).AwayOdds);
                Console.WriteLine("...");
                var tip = network.Run(testingData.Inputs()[i]);

                var success = UIHelpers.SuccessConditionGoalAndPoints(tip, testingData.Outputs()[i]);
                var returns = UIHelpers.BetReturnGoalAndPoints(tip, testingData.Outputs()[i], ((Match)testingData.DataPoints[i].Reference).HomeOdds, ((Match)testingData.DataPoints[i].Reference).AwayOdds);
                money -= 1.00;
                if (success)
                {
                    money += returns * 1.00;
                }
                successes.Add(success);
            }
            Console.WriteLine("Success%: " + successes.Count(x => x) * 1.0 / trainingData.Outputs().Count * 1.0);
            Console.WriteLine("Money: " + money);
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
            Console.WriteLine("Testing");
            Console.WriteLine("Loading data...");
            //Load tipper
            var tipper = new Tipper();

            //Load last completed 
            var year = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year;
            var completedRounds =
                tipper.League.Seasons.Where(s => s.Rounds.Any())
                    .OrderByDescending(s => s.Year)
                    .First()
                    .Rounds.Where(r => r.Matches.All(m => m.TotalScore() > 0))
                    .ToList();
            var round = !completedRounds.Any() ? 0 : completedRounds.OrderByDescending(r => r.Number).First().Number;


            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Last completed year:" + year);
            Console.WriteLine("Last completed round:" + round);

            //Tip
            //Based on Test scenario #670
            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            var trainingData = tipper.GetMatchDataBetween(year - 3, 0, year, round, interpretation);
            trainingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;

            //Create Network
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 250;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");

            //Train Network
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

            //Print results
            Console.WriteLine("Tip {0}...", year);
            var predictions = new List<PredictedMatch>();
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).Where(r => r.Number > round).ToList())
            {
                predictions.AddRange(tipper.PredictWinners(r.Year, r.Number, interpretation));
            }

            var rounds = predictions.Select(p => p.RoundNumber).Distinct();
            foreach (var r in rounds)
            {
                Console.WriteLine("Tip Round {0} ...", r);
                Console.WriteLine(tipper.ResultToString(predictions.Where(p => p.RoundNumber == r).ToList()));
            }
        }
        #endregion
    }
}
