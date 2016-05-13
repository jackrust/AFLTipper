using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArtificialNeuralNetwork.DataManagement;

namespace AustralianRulesFootballBettingOdds
{
    public class BettingData : Data
    {
        public List<double> OddsHome;
        public List<double> OddsAway;
    }
}
