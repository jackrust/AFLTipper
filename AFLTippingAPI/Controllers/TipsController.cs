using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Http;
using AFLStatisticsService;
using ArtificialNeuralNetwork;
using AustralianRulesFootball;
using Tipper.UI;

namespace AFLTippingAPI.Controllers
{
    public class TipsController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            var tips = TipNextRound();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds / 10);

            var output = new List<string> {elapsedTime};
            output.AddRange(tips);

            return output;
        }

        private static List<string> TipNextRound()
        {
            var tips = new List<string>();
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

            //Tip
            var predictions = SetUpTipper(tipper, year, round);
            var rounds = predictions.Select(p => p.RoundNumber).Distinct();
            foreach (var r in rounds)
            {
                var tip = "";
                tip +=  String.Format("Tip Round {0} ...", r);
                tip += String.Format(tipper.ResultToString(predictions.Where(p => p.RoundNumber == r).ToList()));
                tips.Add(tip);
            }
            return tips;
        }

        private static List<PredictedMatch> SetUpTipper(Tipper.Tipper tipper, int year, int round)//Names are getting stupid
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
            tipper.Net.MaxEpochs = 250;//Change back to 750?

            //Train Network
            tipper.Net.Train(trainingData.Inputs(), trainingData.Outputs());

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