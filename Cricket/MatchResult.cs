using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cricket
{
    public enum Victor{
        Home = 0,
        Away = 1,
        Draw = 2,
        Abandoned = 3
    }
    public class MatchResult
    {
        public bool DuckworthLewisStern;
        public Victor Victor;
        public int MarginByRuns;
        public int MarginByWickets;
    }
}
