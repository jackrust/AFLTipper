using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using ArtificialNeuralNetwork.DataManagement;

namespace ArtificialNeuralNetwork
{
    public class Optimizer
    {
        public int UpperLimitLayers;
        public int LowerLimitLayers;
        public int UpperLimitNeuronsInLayer;
        public int LowerLimitNeuronsInLayer;
        public int UpperLimitEpochs;
        public int LowerLimitEpochs;
        public int EpochsStep;
        public double UpperLimitTargetError;
        public double LowerLimitTargetError;
        public double TargetErrorStep;
        public List<TrainingAlgorithmFactory.TrainingAlgorithmType> Algorithms;
        public List<Func<Data, Tuple<Data, Data>>> TrainingTestingDataDelineationCallbacks; 

        public Optimizer()
        {
            UpperLimitLayers = 1;
            LowerLimitLayers = 1;
            UpperLimitNeuronsInLayer = 1;
            LowerLimitNeuronsInLayer = 1;
            UpperLimitEpochs = 1000;
            LowerLimitEpochs = 1000;
            EpochsStep = 250;
            UpperLimitTargetError = 0.005;
            LowerLimitTargetError = 0.005;
            TargetErrorStep = 0.005;
            Algorithms = new List<TrainingAlgorithmFactory.TrainingAlgorithmType>() { TrainingAlgorithmFactory.TrainingAlgorithmType.HoldBestInvestigate };
            // Enum.GetValues(typeof(TrainingAlgorithm.TrainingAlgorithmType)).Cast<Network.TrainingAlgorithm>())
            TrainingTestingDataDelineationCallbacks = new List<Func<Data, Tuple<Data, Data>>>()
            {
                TrainingTestingDataDelineationDefaultImplementation
            };
        }

        public string Optimize(Data data, Func<List<double>, List<double>, bool> successCondition, Func<List<double>, List<double>> deconvert)
        {
            var grapher = new StringBuilder();
            grapher.AppendLine("");
            grapher.AppendLine("Graph data:");
            grapher.AppendLine("id|Layers|Neurons|Epochs|Algorithm|Callback|Success|Time");
            var algorithmCount = 0;
            var callbackCount = 0;

            for (var numLayers = LowerLimitLayers; numLayers < UpperLimitLayers + 1; numLayers++)
            {
                for (var perLayer = LowerLimitNeuronsInLayer; perLayer < (numLayers > 0 ? UpperLimitNeuronsInLayer + 1 : 2); perLayer++)
                {
                    for (var epoch = LowerLimitEpochs; epoch < UpperLimitEpochs + 1; epoch+=EpochsStep)
                    {
                        for (var err = LowerLimitTargetError; err < UpperLimitTargetError + 1; err++)
                        {
                            algorithmCount = 0;
                            foreach (var algorithm in Algorithms )
                            {
                                algorithmCount++;
                                callbackCount = 0;
                                foreach (var dataDelineationCallback in TrainingTestingDataDelineationCallbacks)
                                {
                                    callbackCount++;
                                    grapher.AppendLine(RunTestNetwork(data, successCondition,
                                        deconvert, numLayers, perLayer, epoch,algorithmCount, callbackCount, algorithm, dataDelineationCallback, false));
                                    Console.WriteLine(grapher.ToString());
                                }
                            }
                        }
                    }
                }
            }
            return grapher.ToString();
        }

        public static string RunTestNetwork(Data data, Func<List<double>, List<double>, bool> successCondition, Func<List<double>, List<double>> deconvert, int numLayers, int perLayer, int epoch, int algorithmNumber, int callbackNumber, TrainingAlgorithmFactory.TrainingAlgorithmType algorithm, Func<Data, Tuple<Data, Data>> dataDelineationCallback, bool saveReport = true)
        {
            //Deliniate data
            var dataTuple = dataDelineationCallback(data);
            var trainingData = dataTuple.Item1;
            var testingData = dataTuple.Item2;

            //Create hidden layers
            var hidden = new List<int>();

            for (var i = 0; i < numLayers; i++)
            {
                hidden.Add(perLayer);
            }

            //Create Network
            Network network = new Network(trainingData.DataPoints[0].Inputs.Count, hidden, trainingData.DataPoints[0].Outputs.Count);
            network.MaxEpochs = epoch;
            //Start a stopwatch
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            //Train the network
            network.Train(trainingData.Inputs(), trainingData.Outputs(), algorithm);
            Network.Save(network);

            //Stop the stopwatch
            stopWatch.Stop();

            //Test
            if (saveReport)
            {
                SaveReport(testingData, successCondition, deconvert, network);
            }
            var successes = testingData.Inputs().Select(t => network.Run(t)).Where((result, i) => successCondition(result, testingData.DataPoints[i].Outputs)).Count();

            return String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}", network.Id, numLayers, perLayer, epoch, algorithmNumber, callbackNumber,
               Math.Round((successes / (double)testingData.DataPoints.Count) * 100, 2),
               (double)stopWatch.ElapsedMilliseconds / 1000);
        }

        private static void SaveReport(Data testingData, Func<List<double>, List<double>, bool> successCondition, Func<List<double>, List<double>> deconvert, Network network)
        {
            var report = "";
            for (var i = 0; i < testingData.DataPoints.Count; i++)
            {
                var output = network.Run(testingData.DataPoints[i].Inputs);
                var sccss = successCondition(output, testingData.DataPoints[i].Outputs) ? 1 : 0;
                report = String.Format("{0}|i{1}|o{2}|t{3}|s|{4}\n", i,
                    testingData.DataPoints[i].Inputs.Aggregate(report, (current, inpt) => current + ("|" + inpt)),
                    deconvert(output).Aggregate(report, (current, otpt) => current + ("|" + otpt)),
                    deconvert(testingData.DataPoints[i].Outputs).Aggregate(report, (current, trgt) => current + ("|" + trgt)),
                    sccss);
            }

            using (var file = new System.IO.StreamWriter("Optimizer/report_" + network.Id + ".txt"))
            {
                file.WriteLine(report);
            }
        }

        private static Tuple<Data, Data> TrainingTestingDataDelineationDefaultImplementation(Data data)
        {
            const double trainingRatio = 0.75;
            //scenario is irrelevant here
            var training = new Data();
            var testing = new Data();
            var count = data.DataPoints.Count;

            training.DataPoints = data.DataPoints.GetRange(0, (int) Math.Floor(count*trainingRatio));
            training.SuccessCondition = data.SuccessCondition;
            testing.DataPoints = data.DataPoints.GetRange(training.DataPoints.Count, count - training.DataPoints.Count);
            testing.SuccessCondition = data.SuccessCondition;

            return new Tuple<Data, Data>(training, testing);
        }
    }
}
