﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using AFLStatisticsService;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using AustralianRulesFootball;
using GeneticArtificialNeuralNetwork;
using Tipper.Betting;
using Utilities;
using Team = AustralianRulesFootball.Team;

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
                    case ("B"):
                        TippBBLSeason();
                        break;
                    case ("C"):
                        CompareSetupOptions();
                        break;
                    case ("E"):
                        TipFullSeasonExtended();
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
                        TwitterTipNextRound();
                        break;
                    case ("Z"):
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
            Console.WriteLine("[B]BL tipper");
            Console.WriteLine("Tip with [E]xtended data");
            Console.WriteLine("Tip [F]ull season");
            Console.WriteLine("[G]enetic algorithm");
            Console.WriteLine("[L]ulu");
            Console.WriteLine("Tip [N]ext round");
            Console.WriteLine("[O]ptimise");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("Tip [S]pecific");
            Console.WriteLine("[Z] Testing");
            Console.WriteLine("[?] Show options");
        }
        #endregion

        private static void TipFullSeasonExtended()
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
                    .Rounds.Where(r => r.Matches.Count > 0 && r.Matches.All(m => m.TotalScore() > 0.1))
                    .ToList();

            var round = !completedRounds.Any() ? new RoundShell(year, 0, false) : completedRounds.OrderByDescending(r => r.Number).First();


            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Last completed year:" + round.Year);
            Console.WriteLine("Last completed round:" + round.Number);
            //Tip
            var predictions = SetUpTipper(tipper, round);

            //Save Network
            //var net = tipper.Net;
            //var db = new MongoDb();
            //db.UpdateNetworks(new List<Network>(){net});

            /*var bson = tipper.Net.ToBson();
            var net = BsonSerializer.Deserialize<Network>(bson);

            //var str = Encoding.ASCII.GetString(bson);
            //var bytes = Encoding.ASCII.GetBytes(str);

            string str = Convert.ToBase64String(bson);
            byte[] bytes = Convert.FromBase64String(str);

            var net1 = BsonSerializer.Deserialize<Network>(bytes);*/


            var rounds = predictions.Select(p => p.RoundNumber).Distinct();
            foreach (var r in rounds)
            {
                Console.WriteLine("Tip Round {0} ...", r);
                Console.WriteLine(tipper.ResultToStringAlt(predictions.Where(p => p.RoundNumber == r).ToList()));
            }

            var teams = predictions.Select(p => p.Home).Distinct();
            var ladder = new List<LadderRow>();
            foreach (var team in teams)
            {
                var row = new LadderRow();
                row.team = team;
                row.points = predictions.Count(p => p.IsWinningTeam(team)) * 4;
                row.scoreFor = predictions.Sum(p => p.GetTeamScoreTotal(team));
                row.scoreAgainst = predictions.Sum(p => p.GetOppositionScoreTotal(team));
                ladder.Add(row);
            }

            Console.WriteLine("\nPredicted Ladder");
            Console.WriteLine("Team  | Pts | %");
            foreach (var row in ladder.OrderByDescending(r => r.points).ThenByDescending(r => r.scoreFor / r.scoreAgainst))
            {

                Console.WriteLine("{0, -5} | {1, -3} | {2:N1}", Util.GetTeams().Where(t => t.ApiName == row.team.ApiName).FirstOrDefault().Abbreviation, row.points, (row.scoreFor / row.scoreAgainst) * 100);
            }

        }

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
            var trainingData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(2011, 0, false), new RoundShell(2016, 23, false));
            var testingData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(2016, 0, false), new RoundShell(2018, 23, false));
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

            /*List<List<int>> interpretation = new List<List<int>>
            {
                interpretationTeamScore, interpretationGroundScore, interpretationStateScore, interpretationDayScore, interpretationSharedScore, 
                interpretationTeamShots, interpretationGroundShots, interpretationStateShots, interpretationDayShots, interpretationSharedShots
            };*/
            DataInterpretation interpretation = new DataInterpretation(
            new List<DataInterpretationRule>
            {
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_TEAM, interpretationTeamScore),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_GROUND, interpretationGroundScore),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_STATE, interpretationStateScore),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_DAY, interpretationDayScore),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_SHARED_OPPONENTS, interpretationSharedScore)/*,
                new DataInterpretationRule("Shots By Team", interpretationTeamShots),
                new DataInterpretationRule("Shots By Ground", interpretationGroundShots),
                new DataInterpretationRule("Shots By State", interpretationStateShots),
                new DataInterpretationRule("Shots By Day", interpretationDayShots),
                new DataInterpretationRule("Shots By Recent Shared Opponents", interpretationSharedShots)*/
            });

            var tipper = new Tipper();

            var stopWatch = new Stopwatch();
            var trainingStart = training;
            var trainingEnd = testing - 1;

            const int roundStart = 0;
            const int roundEnd = 23;


            var trainingData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(trainingStart, roundStart, false), new RoundShell(trainingEnd + 1, 0, false), interpretation);
            var testingData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(testing, roundStart, false), new RoundShell(testing, roundEnd, false),  interpretation);
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
            const string options = "Tip [F]ull season, [T]est the algorithm used for tipping, or [B]oth?";
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
                    .Rounds.Where(r => r.Matches.Count > 0 && r.Matches.All(m => m.TotalScore() > 0.1))
                    .ToList();

            var round = !completedRounds.Any() ? new RoundShell(year, 0, false) : completedRounds.OrderByDescending(r => r.Number).First();


            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Last completed year:" + round.Year);
            Console.WriteLine("Last completed round:" + round.Number);
            //Tip
            var predictions = SetUpTipper(tipper, round);

            //Save Network
            var net = tipper.Net;
            var db = new MongoDb();
            db.UpdateNetworks(new List<Network>(){net});


            var rounds = predictions.Select(p => p.RoundNumber).Distinct();


            foreach (var r in rounds)
            {
                Console.WriteLine("Tip Round {0} ...", r);
                Console.WriteLine(tipper.ResultToStringAlt(predictions.Where(p => p.RoundNumber == r).ToList()));
            }

            var completedMatches = completedRounds.SelectMany(r => r.Matches).Where(m => m.Date.Year == predictions.First().Date.Year);

            var teams = predictions.Select(p => p.Home).Distinct();
            var ladder = new List<LadderRow>();
            foreach (var team in teams)
            {
                var row = new LadderRow();
                row.team = team;
                row.points = predictions.Count(p => p.IsWinningTeam(team))*4 + completedMatches.Count(m => m.IsWinningTeam(team)) * 4;
                row.scoreFor = predictions.Sum(p => p.GetTeamScoreTotal(team)) + completedMatches.Sum(m => m.ScoreFor(team).Total());
                row.scoreAgainst = predictions.Sum(p => p.GetOppositionScoreTotal(team)) + completedMatches.Sum(m => m.ScoreAgainst(team).Total());
                ladder.Add(row);
            }

            Console.WriteLine("\nPredicted Ladder");
            Console.WriteLine("Team  | Pts | %");
            foreach (var row in ladder.OrderByDescending(r => r.points).ThenByDescending(r => r.scoreFor/r.scoreAgainst))
            {
                
                Console.WriteLine("{0, -5} | {1, -3} | {2:N1}", Util.GetTeams().Where(t => t.ApiName == row.team.ApiName).FirstOrDefault().Abbreviation, row.points, (row.scoreFor/row.scoreAgainst)*100);
            }

        }

        private class LadderRow
        {
            public Team team;
            public int points;
            public double scoreFor;
            public double scoreAgainst;
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
            var predictions = SetUpTipper(tipper, new RoundShell(year, round, false));
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



        private static List<PredictedMatch> SetUpTipper(Tipper tipper, RoundShell roundShell)//Names are getting stupid
        {
            //Based on Test scenario #670
            /*var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            */

            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };
            //Extension
            /*
            var interpretationQuality = new List<int>();
            var interpretationWins = new List<int>();
            var interpretationKicks = new List<int>() { 5 };
            var interpretationHandballs = new List<int>() { 5 };
            var interpretationMarks = new List<int>() { 5 };
            var interpretationHitouts = new List<int>() { 5 };
            var interpretationTackles = new List<int>() { 5 };
            var interpretationFrees = new List<int>() { 5 };
            */

            /*List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay,
                interpretationShared};//, interpretationQuality, interpretationWins, interpretationKicks, interpretationHandballs, interpretationMarks, interpretationHitouts, interpretationTackles, interpretationFrees};
            */
            DataInterpretation interpretation = new DataInterpretation(
            new List<DataInterpretationRule>
            {
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_TEAM, interpretationTeam),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_GROUND, interpretationGround),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_STATE, interpretationState),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_DAY, interpretationDay),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_SHARED_OPPONENTS, interpretationShared)
            });

            var fromRoundShell = new RoundShell(roundShell.Year - 10, 0, false);
            var trainingData = tipper.GetMatchDataFromLeagueBetween(fromRoundShell, roundShell, interpretation);
            trainingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;

            //Create Network
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 750;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");
            //Console.WriteLine("If it seams fast that's because you forgot to undo the epochs for testing");

            //Train Network
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());
            var str = tipper.Net.Print();
            //var db = new MongoDb();
            //db.UpdateNetworks(new List<Network>() {tipper.Net});

            //Print results
            Console.WriteLine("Tip {0}...", roundShell.Year);
            var predictions = new List<PredictedMatch>();
              foreach (var r in tipper.League.Seasons.Where(s => s.Year == roundShell.Year).SelectMany(s => s.Rounds).Where(r => (r.IsFinal == false && r.EffectiveId() > roundShell.EffectiveId()) || (r.IsFinal == true )).ToList())
            {
                predictions.AddRange(tipper.PredictWinners(r.Year, r.Number, r.IsFinal, interpretation));
            }
            return predictions;
        }
        #endregion

        #region Twitter Tip Next Round
        public static void TwitterTipNextRound()
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
                    .Rounds.Where(r => r.Matches.Count > 0 && r.Matches.All(m => m.TotalScore() > 0.1))
                    .ToList();

            var round = !completedRounds.Any() ? new RoundShell(year, 0, false) : completedRounds.OrderByDescending(r => r.IsFinal).ThenByDescending(r => r.Number).First();

            var nextIncompletRound =
                tipper.League.Seasons.Where(s => s.Rounds.Any() && s.Year >= year)
                    .OrderByDescending(s => s.Year)
                    .First()
                    .Rounds.Where(r => r.Matches.Count > 0 && r.Matches.Where(m => m.TotalScore() < 0.1).Count() > 0)
                    .OrderBy(r => r.EffectiveId())
                    .First();

            Console.WriteLine("Tipping: {0}, " + (nextIncompletRound.IsFinal? "Finals ":"") + " Round {1}", nextIncompletRound.Year, nextIncompletRound.Number);
            //Tip
            var predictions = SetUpTipper(tipper, round);

            var message = "";

            if (nextIncompletRound.IsFinal)
                message = "Finals week " + nextIncompletRound.Number + ":\n";
            else
                message = "Round " + nextIncompletRound.Number + ":\n";

            message += tipper.ResultToStringTweet(predictions.Where(p => p.RoundNumber == nextIncompletRound.Number).ToList());
            Console.WriteLine(message);
            var twitterHelper = new TwitterHelper();
            twitterHelper.SendTweet(message);
            Console.WriteLine("Tweeted successfully");
        }
        #endregion

        //TippBBLSeason
        #region Twitter Tip Season
        private static void TippBBLSeason()
        {
            Console.WriteLine("Loading data...");
            
            //Load tipper
            var tipper = new Tipper();

            //Load last completed 
            var year = tipper.BBLSeasons.Where(s => s.Matches.Any()).OrderByDescending(s => s.Year).First().Year;
            var matches = tipper.BBLSeasons.SelectMany(x => x.Matches).ToList();
            matches.AddRange(tipper.WBBLSeasons.SelectMany(x => x.Matches).ToList());
            var completeMatches = matches.Where(m => m.HomeScore.Runs > 0 || m.AwayScore.Runs > 0 || m.Abandoned);
            var nextIncompletMatch = matches.Where(m => m.HomeScore.Runs == 0 && m.AwayScore.Runs == 0).OrderBy(m => m.EffectiveID()).First();

            Console.WriteLine("Tipping: {0}, Match {1}", nextIncompletMatch.Date.Year, nextIncompletMatch.Number);

            var trainFromID = 201300;
            var testFromID = 201900;

            //Tip
            var trainingData = tipper.GetMatchDataBetween(trainFromID, testFromID);

            //Create Network
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 250;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");

            //Train Network
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());
            var str = tipper.Net.Print();

            //Print results
            Console.WriteLine("Tip BBL from {0}...", testFromID);
            var predictions = new List<Cricket.PredictedMatch>();

            foreach (var r in tipper.BBLSeasons.SelectMany(s => s.Matches).Where(m => m.EffectiveID() >= testFromID).OrderBy(x => x.EffectiveID()).ToList())
            {
                predictions.AddRange(tipper.PredictBBLWinners(r.Date.Year, r.Number));
            }

            Console.WriteLine("Tip WBBL from {0}...", testFromID);
            foreach (var r in tipper.WBBLSeasons.SelectMany(s => s.Matches).Where(m => m.EffectiveID() >= testFromID).OrderBy(x => x.EffectiveID()).ToList())
            {
                predictions.AddRange(tipper.PredictBBLWinners(r.Date.Year, r.Number));
            }

            var message = "";
            var nextMatch = completeMatches.OrderByDescending(m => m.EffectiveID()).First();
             message = "Round " + nextIncompletMatch.Number + ":\n";

            var success = 0.0;
            var failure = 0.0;
            var draws = 0.0;

            foreach (var p in predictions)
            {
                var result = "";
                var m = matches.Where(x => x.EffectiveID() == p.EffectiveID()).First();
                if ((m.HomeWin() && p.HomeWin()) || (m.AwayWin() && p.AwayWin()))
                {
                    success++;
                    result = "Success";
                }
                else if (m.Abandoned || (m.Tie()))
                {
                    draws++;
                    result = "Draw";
                }
                else
                {
                    failure++;
                    result = "Failure";
                }

                Console.WriteLine("{0}| {1,9}: {2,6:P2} - {3,9}: {4,6:P2} ({5,3}-{6,3}) {7}", p.EffectiveID(), p.Home.Names[1], p.HomeProbability, p.Away.Names[1], p.AwayProbability, m.HomeScore.Runs, m.AwayScore.Runs, result);
            }
            Console.WriteLine("Success: {0,10} Failure: {1,10} Draws: {2,12}", success, failure, draws);
            Console.WriteLine("Success: {0,10:P2}", (success/(success+failure+draws)));


            //Optimizing
            var optimizer = new Optimizer();
            optimizer.LowerLimitLayers = 1;
            optimizer.UpperLimitLayers = 3;
            optimizer.LowerLimitNeuronsInLayer = 1;
            optimizer.UpperLimitNeuronsInLayer = 5;
            optimizer.LowerLimitEpochs = 250;
            optimizer.UpperLimitEpochs = 1250;
            optimizer.EpochsStep = 500;
            optimizer.TrainingTestingDataDelineationCallbacks = new List<Func<Data, Tuple<Data, Data>>>()
            {
                DataCallback
            };
            var dataFrom = 201300;
            var dataTo = 202100;

            //Tip
            var data = tipper.GetMatchDataBetween(dataFrom, dataTo);

            var output = optimizer.Optimize(data, UIHelpers.SuccessConditionTotal, UIHelpers.Deconvert);
            Console.WriteLine(output);
            //Console.WriteLine(tipper.Net.Print());
            //var twitterHelper = new TwitterHelper();
            //twitterHelper.SendTweet(message);
            //Console.WriteLine("Tweeted successfully");
        }

        private static Tuple<Data, Data> DataCallback(Data data)
        {
            //scenario is irrelevant here
            var training = new Data();
            var testing = new Data();

            var count = data.DataPoints.Count;
            training.DataPoints = data.DataPoints.GetRange(0, (int)Math.Floor(count * 0.75));
            training.SuccessCondition = data.SuccessCondition;
            testing.DataPoints = data.DataPoints.GetRange((int)Math.Floor(count * 0.75), (int)Math.Floor(count * 0.25));
            testing.SuccessCondition = data.SuccessCondition;

            return new Tuple<Data, Data>(training, testing);
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
            //List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            DataInterpretation interpretation = new DataInterpretation(
            new List<DataInterpretationRule>
            {
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_TEAM, interpretationTeam),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_GROUND, interpretationGround),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_STATE, interpretationState),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_DAY, interpretationDay),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_SHARED_OPPONENTS, interpretationShared)
            });
            var trainingData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(2008, 0, false), new RoundShell(2017, 0, false), interpretation);

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
        /*
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
            var trainingData = tipper.GetMatchDataFromLeagueBetween(2009, 0, 2011, 24);
            Console.WriteLine("Creating testing data...");
            var testingData = tipper.GetMatchDataFromLeagueBetween(2015, 0, 2015, 24);

            Console.WriteLine("Training Neural Network...");
            network.Train(trainingData.Inputs(), trainingData.Outputs());

            Console.WriteLine("Testing Neural Network...");
            var successes = new List<bool>();
            for (var i = 0; i < testingData.Outputs().Count; i++)
            {
                var round = (i / 8) - 1;

                var composite = (Tuple<DateTime, string>)testingData.DataPoints[i].Reference;
                //Attempting to convert from "DataPoint Match reference to tuple reference
                //Realised this wasn't used and commented out for now

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
    */

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
            var year = 2017;//tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year;
            /*var completedRounds =
                tipper.League.Seasons.Where(s => s.Rounds.Any())
                    .OrderByDescending(s => s.Year)
                    .First()
                    .Rounds.Where(r => r.Matches.All(m => m.TotalScore() > 0))
                    .ToList();*/
            var round = 0;// = !completedRounds.Any() ? 0 : completedRounds.OrderByDescending(r => r.Number).First().Number;


            Console.WriteLine("Make sure you've run AFL statistice service.");
            Console.WriteLine("Last completed year:" + year);
            Console.WriteLine("Last completed round:" + round);

            //Tip
            //Based on Test scenario #670
            var interpretationTeam = new List<int> { 1, 2, 3, 5, 8 };
            var interpretationGround = new List<int> { 1, 2, 3, 5, 8 };
            var interpretationState = new List<int> { 1, 2, 3, 5, 8 };
            var interpretationDay = new List<int> { 1, 2, 3, 5, 8 };
            var interpretationShared = new List<int> { 1, 2, 3, 5, 8 };
            //Extension
            var interpretationQuality = new List<int>();
            var interpretationWins = new List<int>();
            var interpretationScores = new List<int>() { 1, 5 };
            var interpretationKicks = new List<int>() { 1, 5 };
            var interpretationHandballs = new List<int>() { 1, 5 };
            var interpretationMarks = new List<int>() { 1, 5 };
            var interpretationHitouts = new List<int>() { 1, 5 };
            var interpretationTackles = new List<int>() { 1, 5 };
            var interpretationFrees = new List<int>() { 1, 5 };


            /*List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay,
                interpretationShared, interpretationQuality, interpretationWins, interpretationKicks, interpretationScores, interpretationHandballs, interpretationMarks,
                interpretationHitouts, interpretationTackles, interpretationFrees};*/
            DataInterpretation interpretation = new DataInterpretation(
            new List<DataInterpretationRule>
            {
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_TEAM, interpretationTeam),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_GROUND, interpretationGround),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_STATE, interpretationState),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_DAY, interpretationDay),
                new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_SHARED_OPPONENTS, interpretationShared),
                /*new DataInterpretationRule(DataInterpretationRuleType.SCORES_BY_QUALITY_OF_OPPONENTS, interpretationQuality),
                new DataInterpretationRule(DataInterpretationRuleType.WINS, interpretationWins),
                new DataInterpretationRule(DataInterpretationRuleType.KICKS, interpretationKicks),
                new DataInterpretationRule(DataInterpretationRuleType.SCORING_SHOTS, interpretationScores),
                new DataInterpretationRule(DataInterpretationRuleType.HANDBALLS, interpretationHandballs),
                new DataInterpretationRule(DataInterpretationRuleType.MARKS, interpretationMarks),
                new DataInterpretationRule(DataInterpretationRuleType.HITOUTS, interpretationHitouts),
                new DataInterpretationRule(DataInterpretationRuleType.TACKLES, interpretationTackles),
                new DataInterpretationRule(DataInterpretationRuleType.FREES, interpretationFrees)*/
            });

            var trainingData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(2012, 0, false), new RoundShell(2017, 0, false), interpretation);
            var testinggData = tipper.GetMatchDataFromLeagueBetween(new RoundShell(2017, 0, false), new RoundShell(2020, 0, false), interpretation);
            trainingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;

            //Investigating training data
            var str = "";
            foreach (var dp in trainingData.DataPoints)
            {
                str = str + String.Join(",", dp.Inputs) + "\n";
            }
            Filey.Save(str, "trainingdata.csv");

            //Create Network
            tipper.Net = Network.CreateNetwork(trainingData, 1, 3, TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate);
            tipper.Net.MaxEpochs = 250;
            Console.WriteLine("Training network (" + tipper.Net.Id + ")...");

            //Train Network
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

            //Print results
            Console.WriteLine("Tip {0}...", year);
            var predictions = new List<PredictedMatch>();
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == 2017).SelectMany(s => s.Rounds).Where(r => r.Number > 0).ToList())
            {
                predictions.AddRange(tipper.PredictWinners(r.Year, r.Number, r.IsFinal, interpretation));
            }
            var successes = new List<bool>();

            foreach (var p in predictions)
            {
                var actual = tipper.League.Seasons.SelectMany(s => s.Rounds).SelectMany(x => x.Matches).Where(m => m.Date == p.Date && m.Home == p.Home && m.Away == p.Away).FirstOrDefault();

                Console.WriteLine("Tip {0}, {1} ...", p.Date.Year, p.RoundNumber);
                Console.WriteLine("{0} - {1} | {2} - {3}", p.HomeTotal, p.AwayTotal, actual.HomeScore().Total(), actual.AwayScore().Total());

                var success = false;
                if (p.HomeTotal > p.AwayTotal && actual.HomeScore().Total() > actual.AwayScore().Total())
                    success = true;
                if (p.HomeTotal < p.AwayTotal && actual.HomeScore().Total() < actual.AwayScore().Total())
                    success = true;
                if (Math.Abs(p.HomeTotal - p.AwayTotal) < 0.1 && Math.Abs(actual.HomeScore().Total() - actual.AwayScore().Total()) < 0.1)
                    success = true;
                successes.Add(success);
            }
            Console.WriteLine("Correct: {0}, Incorrect: {1}, Success rate: {2}", successes.Count(s => s), successes.Count(s => !s), (double)successes.Count(s => s) / (double)successes.Count);
        }
        #endregion
    }
}
