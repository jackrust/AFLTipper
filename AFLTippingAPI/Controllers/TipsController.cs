using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using AFLStatisticsService;
using AFLTippingAPI.Controllers.Statistics;
using AFLTippingAPI.Helpers;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using MongoDB.Bson;
using Tipper.UI;

namespace AFLTippingAPI.Controllers
{
    public class TipsController : ApiController
    {
        private readonly MongoDb _db;

        public TipsController()
        {
            _db = new MongoDb();
        }

        // GET api/values
        public string Get()
        {
            var predictions = JustTipFullSeason();
            var simplePredictions = SimplePrediction.Convert(predictions);
            return simplePredictions.ToJson();
        }

        private List<PredictedMatch> JustTipFullSeason()
        {
            //Load tipper
            var tipper = new Tipper.Tipper();

            //Load last completed 
            var league = tipper.League;
            var seasons = league.Seasons;
            var filteredSeasons = seasons.Where(s => s.Rounds.Any());
            var orderedfilteredseasons = filteredSeasons.OrderByDescending(s => s.Year);
            var first = orderedfilteredseasons.First().Year;
            var year = tipper.League.Seasons.Where(s => s.Rounds.Any()).OrderByDescending(s => s.Year).First().Year;
            var completedRounds =
                tipper.League.Seasons.Where(s => s.Rounds.Any())
                    .OrderByDescending(s => s.Year)
                    .First()
                    .Rounds.Where(r => r.Matches.All(m => m.TotalScore() > 0))
                    .ToList();
            var round = !completedRounds.Any() ? 0 : completedRounds.OrderByDescending(r => r.Number).First().Number;

            //Tip
            return SetUpTipper(tipper, year, round);

        }

        private List<PredictedMatch> SetUpTipper(Tipper.Tipper tipper, int year, int round)//Names are getting stupid
        {
            
            //Based on Test scenario #670. If this changes the network will need to change too.
            var interpretationTeam = new List<int> { 9, 13, 17 };
            var interpretationGround = new List<int> { 25, 31, 37 };
            var interpretationState = new List<int> { 1, 3, 5 };
            var interpretationDay = new List<int> { 25, 31, 37 };
            var interpretationShared = new List<int> { 25, 31, 37 };

            List<List<int>> interpretation = new List<List<int>> { interpretationTeam, interpretationGround, interpretationState, interpretationDay, interpretationShared };
            /*
            var trainingData = tipper.GetMatchDataFromLeagueBetween(year - 10, 0, year, round, interpretation);
            trainingData.SuccessCondition = UIHelpers.SuccessConditionTotalPrint;
            */


            //Load Network
            var network = _db.GetNetworks().ToList();
            var first = network.First(n => n.Id == Global.NeuralNetworkId);
            //Neurons don't seem to plug themselves automatically after being stored.
            Network.PlugIn(first.ONeurons, first.HLayers, first.INeurons);
            tipper.Net = first;
            //Print results
            var predictions = new List<PredictedMatch>();
            foreach (var r in tipper.League.Seasons.Where(s => s.Year == year).SelectMany(s => s.Rounds).Where(r => r.Number > round).ToList())
            {
                predictions.AddRange(tipper.PredictWinners(r.Year, r.Number, interpretation));
            }
            return predictions;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
            //Update Seasons
            var seasonsController = new SeasonsController();
            seasonsController.Update();
            //Update Data Interpretation
            var interpretedDataController = new InterpretedDataController();
            interpretedDataController.Update();

        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}