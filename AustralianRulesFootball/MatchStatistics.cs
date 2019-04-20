using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustralianRulesFootball
{
    public class MatchStatistics : Entity
    {
        public string For { get; set; }
        public string Against { get; set; }
        public int Year { get; set; }
        public int RoundNo { get; set; }
        public int Kicks { get; set; }
        public int Handballs { get; set; }
        public int Marks { get; set; }
        public int HitOuts { get; set; }
        public int Tackles { get; set; }
        public int FreesFor { get; set; }
        public int FreesAgainst { get; set; }
    }
}
