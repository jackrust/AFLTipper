using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tipper.Betting
{
    public class BettingRule
    {
        public int Priority;
        public double Wager;
        public Func<double, bool> Scenario { get; set; }
    }
}
