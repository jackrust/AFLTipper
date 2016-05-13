using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AustralianRulesFootballBettingOdds;

namespace AFLBettingTestHarness
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var odds = MatchOdds.LoadMatchOddsList();
        }
    }
}
