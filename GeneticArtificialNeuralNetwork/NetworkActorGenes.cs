using System;
using System.Collections.Generic;
using System.Linq;
using ArtificialNeuralNetwork.DataManagement;

namespace GeneticArtificialNeuralNetwork
{
    public class NetworkActorGenes
    {
        private Random _random;
        public List<double> Facade;
        public static int MinHiddenLayers = 1;
        public static int MaxHiddenLayers = 2;
        public List<double> HiddenLayers;//should be int
        public static int MinHiddenNeurons = 1;
        public static int MaxHiddenNeurons = 5;
        public List<List<double>> HiddenNeurons;//should be int

        public NetworkActorGenes()
        {
            _random = new Random();
            Facade = new List<double>();
            HiddenLayers = new List<double>();
            HiddenNeurons = new List<List<double>>();
        }

        public NetworkActor GenerateActor(int outputs)
        {
            return new NetworkActor(GenerateRandomDataSubset(), GenerateRandomHiddenLayerDefinition(),
                        outputs);
        }

        public DataFacadeGrouped GenerateRandomDataSubset()
        {
            var dataSubset = Facade.Select(t => _random.NextDouble() < t).ToList();

            var facade = new DataFacadeGrouped();
            facade.SetMask(dataSubset);
            return facade;
        }

        public List<int> GenerateRandomHiddenLayerDefinition()
        {
            //Number of Layers
            var rh = _random.Next(0, HiddenLayers.Count);
            var numLayers = HiddenLayers[rh];

            //Neurons in Layer

            var definition = new List<int>();

            for (var i = 0; i < numLayers; i++)
            {
                var rn = _random.Next(0, HiddenNeurons[i].Count);
                definition.Add(rn);
            }

            return definition;
        }

        public static NetworkActorGenes GenerateRepresentative(List<NetworkActor> actors, Random random)
        {
            var representative = new NetworkActorGenes();
            if (actors == null || actors.Count == 0)
                return representative;

            var actorsSize = actors.Count;

            //Facade
            var maskSize = actors[0].Facade.GetMask().Count;
            var average = (actors.Sum(a => a.Facade.Count) * 1.0) / (actors.Sum(a => a.Facade.GetMask().Count) * 1.0);
            var relativeValue = average/(actorsSize + 2.0);
            var facadeHeatMap = new List<double>();

            for (var i = 0; i < maskSize; i++)
            {
                //If everyone has the item we still want less that 100% chance. Same for 0%
                facadeHeatMap.Add(relativeValue * (0.0 + 1.0 + actors.Count(a => a.Facade.GetMask()[i])));
            }

            //Layers
            var hiddenLayers = new List<double>();
            for (int i = MinHiddenLayers; i < MaxHiddenLayers; i++)
            {
                //Everyone should have at least a little chance
                hiddenLayers.Add(i);

                var count = actors.Count(a => a.Network.HLayers.Count == i);
                for (var j = 0; j < count; j++)
                {
                    hiddenLayers.Add(i);
                }
            }
            

            //Neurons
            //TODO Redo this - each option should have it's own weight, weighted answer comes from the chance a ranomd number hits and instance of a given number
            var hiddenNeurons = new List<List<double>>();
            for (var i = MinHiddenLayers; i < MaxHiddenLayers; i++)
            {
                var hiddenNeuronsInLayer = new List<double>();
                for (var j = MinHiddenNeurons; j < MaxHiddenNeurons; j++)
                {
                    //Everyone should have at least a little chance
                    hiddenNeuronsInLayer.Add(j);

                    var count = actors.Count(a => a.Network.HLayers.Count > i && a.Network.HLayers[i].Count == j);
                    for (var k = 0; k < count; k++)
                    {
                        hiddenNeuronsInLayer.Add(i);
                    }
                }
                hiddenNeurons.Add(hiddenNeuronsInLayer);
            }

            //Return
            representative._random = random;
            representative.Facade = facadeHeatMap;
            representative.HiddenLayers = hiddenLayers;
            representative.HiddenNeurons = hiddenNeurons;
            return representative;
        }

        public static NetworkActorGenes GenerateRandomRepresentative(Random random)
        {
            var representative = new NetworkActorGenes();
            //TODO: so many magic numbers
            //Facade
            const double average = 12.0/48.0;
            var facadeHeatMap = new List<double>();

            for (var i = 0; i < 48; i++)
            {
                //If everyone has the item we still want less that 100% chance. Same for 0%
                facadeHeatMap.Add(average);
            }

            //Layers
            var hiddenLayers = new List<double>();
            for (int i = MinHiddenLayers; i < MaxHiddenLayers; i++)
            {
                //Everyone should have at least a little chance
                hiddenLayers.Add(i);
            }


            //Neurons
            //TODO: Redo this - each option should have it's own weight, weighted answer comes from the chance a ranomd number hits and instance of a given number
            var hiddenNeurons = new List<List<double>>();
            for (var i = 0; i < MaxHiddenLayers; i++)
            {
                var hiddenNeuronsInLayer = new List<double>();
                for (var j = 0; j < MaxHiddenNeurons; j++)
                {
                    //Everyone should have at least a little chance
                    hiddenNeuronsInLayer.Add(j);
                }
                hiddenNeurons.Add(hiddenNeuronsInLayer);
            }

            //Return
            representative._random = random;
            representative.Facade = facadeHeatMap;
            representative.HiddenLayers = hiddenLayers;
            representative.HiddenNeurons = hiddenNeurons;
            return representative;
        }
    }
}
