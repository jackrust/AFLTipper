using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustralianRulesFootball
{
    public class PredictedMatch : Match
    {
        public double HomeTotal = 0;
        public double AwayTotal = 0;
        public int RoundNumber;

        public PredictedMatch(Team home, Score hq1, Score hq2, Score hq3, Score hq4, Team away, Score aq1, Score aq2,
            Score aq3,
            Score aq4, Ground ground, DateTime date)
            : base(home, hq1, hq2, hq3, hq4, away, aq1, aq2, aq3,
            aq4, ground, date)
        {}

        public PredictedMatch(Team home, Team away, List<Quarter> quarters, Ground ground, DateTime date,
            double homeOdds, double awayOdds) : base(home, away, quarters, ground, date, homeOdds, awayOdds)
        {}

        public PredictedMatch(Team home, Team away, Ground ground, DateTime date, double homeTotal, double awayTotal, int roundNumber)
            : base(home, new Score(), new Score(), new Score(), new Score(), away, new Score(), new Score(), new Score(),
                new Score(), ground, date)
        {
            HomeTotal = homeTotal;
            AwayTotal = awayTotal;
            RoundNumber = roundNumber;
        }

        public double Total()
        {
            return HomeTotal + AwayTotal;
        }
    }
}
