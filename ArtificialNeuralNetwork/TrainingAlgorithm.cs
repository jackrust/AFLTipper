using System;
using System.Collections.Generic;
using System.Linq;
using Utilities;

namespace ArtificialNeuralNetwork
{
    public class TrainingAlgorithmFactory
    {
        public enum TrainingAlgorithmType
        {
            Normal,
            HoldBest,
            HoldBestNarrowLearning,
            HoldBestInvestigate,
            HoldBestCullOutliersInvestigate
        }

        public static TrainingAlgorithm CreateAlgoRithm(TrainingAlgorithmType type)
        {
            TrainingAlgorithm algorithm;
            switch (type)
            {
                case (TrainingAlgorithmType.Normal):
                    algorithm = new TrainingAlgorithmNormal();
                    break;
                case (TrainingAlgorithmType.HoldBest):
                    algorithm = new TrainingAlgorithmHoldBest();
                    break;
                case (TrainingAlgorithmType.HoldBestNarrowLearning):
                    algorithm = new TrainingAlgorithmHoldBestNarrowLearning();
                    break;
                case (TrainingAlgorithmType.HoldBestInvestigate):
                    algorithm = new TrainingAlgorithmHoldBestInvestigate();
                    break;
                case (TrainingAlgorithmType.HoldBestCullOutliersInvestigate):
                    algorithm = new TrainingAlgorithmHoldBestCullOutliersInvestigate();
                    break;
                default:
                    algorithm = new TrainingAlgorithmNormal();
                    break;
            }
            return algorithm;
        }
    }

    public abstract class TrainingAlgorithm
    {
        public abstract void Train(Network network, List<List<double>> inputs, List<List<double>> targets);

        protected static void AdjustLearningRateDown(Network network)
        {
            foreach (var h in network.HLayers.SelectMany(l => l))
            {
                h.HalveLearningRate();
            }
            foreach (var o in network.ONeurons)
            {
                o.HalveLearningRate();
            }
        }
    }

    public class TrainingAlgorithmNormal : TrainingAlgorithm
    {
        public override void Train(Network network, List<List<double>> inputs, List<List<double>> targets)
        {
            network.Epochs = 0;
            do
            {
                network.Error = network.TrainEpoch(inputs, targets);
                network.Epochs++;
            } while (network.Error > network.TargetError && network.Epochs < network.MaxEpochs);
        }
    }

    public class TrainingAlgorithmHoldBest : TrainingAlgorithm
    {
        public override void Train(Network network, List<List<double>> inputs, List<List<double>> targets)
        {
            network.Epochs = 0;
            var minima = 0;
            double bestError = -1;
            var bestWeights = network.GetWeights();
            do
            {
                network.Error = network.TrainEpoch(inputs, targets);
                network.Epochs++;
                minima++;
                if (network.Error < bestError || bestError < 0)
                {
                    minima = 0;
                    bestError = network.Error;
                    bestWeights = network.GetWeights();
                }
            } while (network.Error > network.TargetError && minima < network.MaxMinima &&
                     network.Epochs < network.MaxEpochs);
            network.SetWeights(bestWeights);
        }
    }

    public class TrainingAlgorithmHoldBestNarrowLearning : TrainingAlgorithm
    {
        public override void Train(Network network, List<List<double>> inputs, List<List<double>> targets)
        {
            network.Epochs = 0;
            var minima = 0;
            double minError = -1;
            double maxError = -1;
            double prevError = -1;

            var bestWeights = network.GetWeights();
            do
            {
                network.Error = network.TrainEpoch(inputs, targets)/inputs.Count;
                network.Epochs++;
                minima++;

                if (network.Error < minError || minError < 0)
                {
                    minima = 0;
                    minError = network.Error;
                    bestWeights = network.GetWeights();
                }

                if (network.Error > maxError)
                {
                    maxError = network.Error;
                }

                if (network.Error > prevError)
                {
                    AdjustLearningRateDown(network);
                }
                prevError = network.Error;
            } while (network.Error > network.TargetError && minima < network.MaxMinima &&
                     network.Epochs < network.MaxEpochs);
            network.SetWeights(bestWeights);
        }
    }

    public class TrainingAlgorithmHoldBestInvestigate : TrainingAlgorithm
    {
        public override void Train(Network network, List<List<double>> inputs, List<List<double>> targets)
        {
            network.Epochs = 0;
            var minima = 0;
            double minError = -1;
            double maxError = -1;
            double prevError = -1;
            var log = new List<List<double>>();
            do
            {

                network.Error = network.TrainEpoch(inputs, targets) / inputs.Count;
                network.Epochs++;
                minima++;

                if (network.Error < minError || minError < 0)
                {
                    minima = 0;
                    minError = network.Error;
                    //Network.Save(network); TOTO:reinstate using mongo
                }

                if (network.Error > maxError)
                {
                    maxError = network.Error;
                }

                if (network.Error > prevError)
                {
                    AdjustLearningRateDown(network);
                }
                prevError = network.Error;
                log.Add(new List<double>() {network.Epochs, minima, network.Error, minError, maxError});

            } while (minima < network.MaxMinima && network.Epochs < network.MaxEpochs);

            //TODO: reinstate with mongoDB
            //network = Network.Load(network.Directory + network.Id + ".ann");
            //Filey.Save(log, "Network/Algorithm/Log.txt");
            //var rankings = network.RankInputs();
            //Filey.Save(rankings, "Network/Algorithm/Rankings.txt");
        }
    }

    public class TrainingAlgorithmHoldBestCullOutliersInvestigate : TrainingAlgorithm
    {
        public override void Train(Network network, List<List<double>> inputs, List<List<double>> targets)
        {
            network.Epochs = 0;
            var minima = 0;
            double minError = -1;
            double maxError = -1;
            double prevError = -1;
            const double cycleLength = 100;
            double cullPopulation = 50;
            var log = new List<List<double>>();
            do
            {
                network.Error = network.TrainEpoch(inputs, targets) / inputs.Count;
                network.Epochs++;
                minima++;

                if (network.Error < minError || minError < 0)
                {
                    minima = 0;
                    minError = network.Error;
                    //Network.Save(network); //TODO: reinstate using mongodb
                }

                if (network.Error > maxError)
                {
                    maxError = network.Error;
                }

                if (network.Error > prevError)
                {
                    AdjustLearningRateDown(network);
                }
                prevError = network.Error;
                log.Add(new List<double>() { network.Epochs, minima, network.Error, minError, maxError });

                //Cull outliers
                var largestError = 0;
                var populationErrors = new Dictionary<int, double>();
                var temp = "Number,Error\n";
                if (network.Epochs % cycleLength == 0)
                {
                    //for each of the training cases
                    for (var i = 0; i < inputs.Count; i++)
                    {
                        //get the output List for the given input List
                        var outputs = network.Run(inputs[i]);
                        var error = outputs.Select((t, j) => Math.Abs(targets[i][j] - t)).Sum();
                        populationErrors.Add(i, error);
                    }

                    temp = populationErrors.Aggregate(temp, (current, p) => current + ("" + p.Key + "," + p.Value + "\n"));
                    Filey.Save(temp, "tamp_ANNOutput.txt");
                    var boundary = LocatePercentileError(0.995, populationErrors.Select(e => e.Value).ToList());
                    var cullInputs = populationErrors.Where(p => p.Value > boundary).Select(e => inputs[e.Key]);
                    var cullTargets = populationErrors.Where(p => p.Value > boundary).Select(e => targets[e.Key]);
                    foreach (var i in cullInputs)
                    {
                        inputs.Remove(i);
                    }
                    foreach (var t in cullTargets)
                    {
                        targets.Remove(t);
                    }
                }
            } while (minima < network.MaxMinima && network.Epochs < network.MaxEpochs);

            network = Network.Load(network.Directory + network.Id + ".ann");
            Filey.Save(log, "Network/Algorithm/Log.txt");
            var rankings = network.RankInputs();
            Filey.Save(rankings, "Network/Algorithm/Rankings.txt");
        }

        private double LocatePercentileError(double percentile, List<double> errors)
        {
            var boundaryError = 0;
            var index = (int)Math.Ceiling(percentile*errors.Count());
            var sorted = errors.OrderBy(e => e).ToList();
            return sorted[index];
        }
    }
}
