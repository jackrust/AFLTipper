using AustralianRulesFootball;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cricket
{
    public class PredictedMatch : Match
    {
        public double HomeProbability;
        public double AwayProbability;

        public PredictedMatch(Team home, Team away, Ground ground, DateTime date, int number, double v1, double v2)
        {
            Home = home;
            Away = away;
            Ground = ground;
            Date = date;
            Number = number;
            HomeProbability = v1;
            AwayProbability = v2;
        }

        public new bool HomeWin()
        {
            return HomeProbability > AwayProbability;
        }

        public new bool AwayWin()
        {
            return AwayProbability > HomeProbability;
        }
    }
}
