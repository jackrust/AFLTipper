using System;
using System.Collections.Generic;
using System.Linq;
using AustralianRulesFootball;

namespace AFLTippingAPI.Controllers
{
    public class SimplePrediction
    {
        public string Home;
        public string Away;
        public string Ground;
        public DateTime Date;
        public double HomeTotal;
        public double AwayTotal;
        public int Year;
        public int RoundNumber;

        public static List<SimplePrediction> Convert(List<PredictedMatch> predictedMatches)
        {
            var simplePredictions = new List<SimplePrediction>();
            foreach (var predictedMatch in predictedMatches)
            {
                var simplePrediction = new SimplePrediction()
                {
                    Home = predictedMatch.Home.Region,
                    Away = predictedMatch.Away.Region,
                    Ground = predictedMatch.Ground.Names.First(),
                    Date = predictedMatch.Date,
                    HomeTotal = predictedMatch.HomeTotal,
                    AwayTotal = predictedMatch.AwayTotal,
                    Year = predictedMatch.Date.Year,
                    RoundNumber = predictedMatch.RoundNumber
                };
                simplePredictions.Add(simplePrediction);
            }

            return simplePredictions;
        }
    }
}