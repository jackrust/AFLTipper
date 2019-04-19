using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using ArtificialNeuralNetwork;
using ArtificialNeuralNetwork.DataManagement;
using GeneticAlgorithm;
using Utilities;

namespace GeneticArtificialNeuralNetwork
{
    public class NetworkActor : Actor
    {
        public string Id = Guid.NewGuid().ToString();
        public Network Network;
        public List<Network> OldNetworks;
        public DataFacadeGrouped Facade;
        public double SuccessRate;
        public long TimeToTrain;
        public long TimeToTest;

        public NetworkActor()
        {
            Facade = new DataFacadeGrouped();
            Generations = new List<int>();
            OldNetworks = new List<Network>();
        }

        public NetworkActor(DataFacadeGrouped facade, IReadOnlyCollection<int> hiddens, int outputs)
        {
            Facade = facade;
            Generations = new List<int>();
            OldNetworks = new List<Network>();
            Network = new Network(facade.Count, hiddens, outputs);
            //TODO: remove, and add in to Genes as a randomisable trait
            Network.MaxEpochs = 500;
        }

        public void RefreshNetwork()
        {
            OldNetworks.Add(Network);
            var iNeurons = Network.INeurons.Count;
            var hLayers = Network.HLayers.Select(t => t.Count).ToList();
            var oNeurons = Network.ONeurons.Count;


            Network = new Network(iNeurons, hLayers, oNeurons);
        }

        public void Train(Data data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Facade.SetData(data);
            var subset = Facade.GetData();
            Network.Train(subset.Inputs(), subset.Outputs());

            stopwatch.Stop();
            TimeToTrain = stopwatch.ElapsedMilliseconds;
        }

        public virtual void Test(Data data)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Facade.SetData(data);
            var subset = Facade.GetData();
            var successes = subset.Inputs().Select(t => Network.Run(t)).Where((result, i) => subset.SuccessCondition(result, subset.DataPoints[i].Outputs, null)).Count();
            SuccessRate = 100 * (double)successes / subset.DataPoints.Count;

            stopwatch.Stop();
            TimeToTest = stopwatch.ElapsedMilliseconds;
        }


        public override double GetFitness()
        {
            return SuccessRate;
        }

        public static NetworkActor BestGuessNetworkActor()
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
            return new NetworkActor(controlFacade, new List<int> { hiddens }, outputs);
        }

        #region IO
        public String Stringify()
        {
            var s = "";
            s += "<id>" + Id + "</id>";
            s += "<networkId>" + Network.Id + "</networkId>";
            s += "<oldNetworkIds>" + string.Join(",", OldNetworks.Select(n => n.Id)) + "</oldNetworkIds>";
            s += "<datasubset>" + string.Join(",", Facade.GetMask().Select(x => x.ToString()).ToArray()) + "</datasubset>";
            s += "<successRate>" + SuccessRate + "</successRate>";
            return s;
        }

        public static NetworkActor Objectify(string str)
        {
            var networkId = Stringy.SplitOn(str, "network")[0];
            var oldNetworkIds = Stringy.SplitOn(str, "oldNetworkIds")[0];
            var datasubsetString = Stringy.SplitOn(str, "datasubset")[0];
            var datasubset = datasubsetString.Split(',').Select(bool.Parse).ToList();
            var successRate = Double.Parse(Stringy.SplitOn(str, "successRate")[0]);

            var actor = new NetworkActor {Network = Network.Load("Network/" + networkId + ".ann")};
            var oldNetworks = oldNetworkIds.Select(oldNetworkId => Network.Load("Network/" + oldNetworkId + ".ann")).ToList();
            actor.Facade.SetMask(datasubset);
            actor.SuccessRate = successRate;
            actor.OldNetworks = oldNetworks;
            return actor;
        }
        #endregion
    }
}
