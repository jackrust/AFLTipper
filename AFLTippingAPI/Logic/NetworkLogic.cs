using System.Collections.Generic;
using System.Linq;
using AFLStatisticsService;
using AFLTippingAPI.Helpers;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Tipper;

namespace AFLTippingAPI.Logic
{
    public class NetworkLogic
    {
        public static List<PredictedMatch> JustTipFullSeason()
        {
            var db = new MongoDb();

            //Load tipper
            var tipper = new Tipper.Tipper();

            //Load last completed
            var year = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year;
            var completedRounds =
                tipper.League.Seasons.Where(s => s.Rounds.Any())
                    .OrderByDescending(s => s.Year)
                    .First()
                    .Rounds.Where(r => r.Matches.All(m => m.TotalScore() > 0))
                    .ToList();
            var round = !completedRounds.Any() ? 0 : completedRounds.OrderByDescending(r => r.Number).First().Number;

            //Load Network
            var network = db.GetNetworks().ToList();
            var first = network.First(n => n.Id == Global.NeuralNetworkId);

            //Neurons don't seem to plug themselves automatically after being stored.
            Network.PlugIn(first.ONeurons, first.HLayers, first.INeurons);
            tipper.Net = first;

            //Print results
            var predictions = new List<PredictedMatch>();
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).Where(r => r.Number > round).ToList())
            {
                //If Interpretation change network will need to change too
                predictions.AddRange(tipper.PredictWinners(r.Year, r.Number, AFLDataInterpreter.Interpretations.BespokeApiInterpretation));
            }
            return predictions;
        }
    }
}