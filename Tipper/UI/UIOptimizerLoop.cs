using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;

namespace Tipper.UI
{
    public class UIOptimizerLoop
    {
        public static void LoadOptimizerLoop()
        {
            var loop = true;
            const string options = "Run [O]ptimizer, Run the [M]eta Optimizer to test data interpereter options";
            Console.WriteLine("Cool, I can tip the full season four you.");
            Console.WriteLine(options);

            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("O"):
                        RunOptimizer();
                        break;
                    case ("M"):
                        RunMetaOptimizer();
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

        #region optimiser

        private static void RunOptimizer()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            Console.WriteLine("Creating Optimizer...");
            var optimizer = new Optimizer();
            var output = "";

            optimizer.LowerLimitLayers = 1;
            optimizer.UpperLimitLayers = 1;
            optimizer.LowerLimitNeuronsInLayer = 3;
            optimizer.UpperLimitNeuronsInLayer = 3;
            optimizer.LowerLimitEpochs = 250;
            optimizer.UpperLimitEpochs = 1250;
            optimizer.EpochsStep = 500;
            optimizer.TrainingTestingDataDelineationCallbacks = new List<Func<Data, Tuple<Data, Data>>>()
            {
                OneYearAgoTenYearsTrained,
                TwoYearAgoTenYearsTrained,
                ThreeYearAgoTenYearsTrained,
                FourYearAgoTenYearsTrained
            };


            Console.WriteLine("Loading data...");
            var data = tipper.GetMatchDataFromLeagueBetween(2005, 0, 2019, 0);

            var count = 0;
            foreach (var dataPoint in data.DataPoints)
            {
                foreach (var input in dataPoint.Inputs)
                {
                    if (input < 0.0001)
                        count++;
                }
            }
            Console.WriteLine("Zeros: {0}", count);
            Console.WriteLine("Optimizing...");
            output = optimizer.Optimize(data, UIHelpers.SuccessConditionTotal, UIHelpers.Deconvert);

            Console.WriteLine(output);
        }

        private static Tuple<Data, Data> OneYearAgoTenYearsTrained(Data data)
        {
            return LoadLastThreeYearsFromXYearsAgo(data, 1, 10);
        }

        private static Tuple<Data, Data> TwoYearAgoTenYearsTrained(Data data)
        {
            return LoadLastThreeYearsFromXYearsAgo(data, 2, 10);
        }

        private static Tuple<Data, Data> ThreeYearAgoTenYearsTrained(Data data)
        {
            return LoadLastThreeYearsFromXYearsAgo(data, 3, 10);
        }

        private static Tuple<Data, Data> FourYearAgoTenYearsTrained(Data data)
        {
            return LoadLastThreeYearsFromXYearsAgo(data, 4, 9);
        }

        private static Tuple<Data, Data> LoadLastThreeYearsFromXYearsAgo(Data data, int testingYearsAgo,
            int numTrainingYears)
        {
            int gamesPerSeason = 207;
            //scenario is irrelevant here
            var training = new Data();
            var testing = new Data();
            var count = data.DataPoints.Count;

            training.DataPoints = data.DataPoints.GetRange(
                count - (gamesPerSeason*(numTrainingYears + testingYearsAgo)), (gamesPerSeason*numTrainingYears));
            training.SuccessCondition = data.SuccessCondition;
            testing.DataPoints = data.DataPoints.GetRange(count - (gamesPerSeason*testingYearsAgo), gamesPerSeason);
            testing.SuccessCondition = data.SuccessCondition;

            return new Tuple<Data, Data>(training, testing);
        }

        #endregion

        #region MetaOptimizer

        private static void RunMetaOptimizer()
        {
            Console.WriteLine("Start");
            Console.WriteLine("Creating Tipper...");
            var tipper = new Tipper();
            Console.WriteLine("Creating Optimizer...");
            var optimizer = new Optimizer();
            var output = "";

            optimizer.LowerLimitLayers = 1;
            optimizer.UpperLimitLayers = 1;
            optimizer.LowerLimitNeuronsInLayer = 3;
            optimizer.UpperLimitNeuronsInLayer = 3;
            optimizer.LowerLimitEpochs = 750;
            optimizer.UpperLimitEpochs = 750;
            optimizer.EpochsStep = 500;
            optimizer.TrainingTestingDataDelineationCallbacks = new List<Func<Data, Tuple<Data, Data>>>()
            {
                OneYearAgoTenYearsTrained,
                TwoYearAgoTenYearsTrained,
                ThreeYearAgoTenYearsTrained,
                FourYearAgoTenYearsTrained
            };




            Console.WriteLine("Optimizing...");
            var optimizationDataSetOne = new List<List<int>>
            {
                new List<int> {0},
                new List<int> {1},
                new List<int> {5},
                new List<int> {21}
            };
            var optimizationDataSetTwo = new List<List<int>>
            {
                new List<int> {0},
                new List<int> {1},
                new List<int> {5},
                new List<int> {21}
            };
            var optimizationDataSetThree = new List<List<int>>
            {
                new List<int> {0},
                new List<int> {1},
                new List<int> {5},
                new List<int> {21}
            };
            var optimizationDataSetFour = new List<List<int>>
            {
                new List<int> {0},
                new List<int> {1},
                new List<int> {5},
                new List<int> {21}
            };
            var optimizationDataSetFive = new List<List<int>>
            {
                new List<int> {0},
                new List<int> {1},
                new List<int> {5},
                new List<int> {21}
            };
            foreach (var one in optimizationDataSetOne)
            {
                foreach (var two in optimizationDataSetTwo)
                {
                    foreach (var three in optimizationDataSetThree)
                    {
                        foreach (var four in optimizationDataSetFour)
                        {
                            foreach (var five in optimizationDataSetFive)
                            {

                                var interperetation = new List<List<int>>
                                {
                                    one,
                                    two,
                                    three,
                                    four,
                                    five
                                };
                                if (interperetation.Sum(i => i.Sum()) > 0)
                                {
                                    Console.WriteLine("Loading data...");
                                    var data = tipper.GetMatchDataFromLeagueBetween(2003, 0, 2019, 0, interperetation);
                                    Console.WriteLine("Optimizing...{0}-{1}-{2}-{3}-{4}", one[0], two[0], three[0],
                                        four[0],
                                        five[0]);
                                    optimizer.Optimize(data, UIHelpers.SuccessConditionTotal, UIHelpers.Deconvert);
                                }
                            }
                        }
                    }
                }
            }
            //0- 0-21-21-21- 1 gives 2018 =74%
            //1- 0- 0- 1-21- 1 gives 2018 =66%
            //1- 0- 0- 0-21- 1 gives 2018 =66%
            #endregion
        }
    }
}
