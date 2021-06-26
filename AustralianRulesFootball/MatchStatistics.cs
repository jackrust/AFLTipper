using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AustralianRulesFootball
{
    public class MatchStatistics : Entity
    {
        //Min-max taken from 2000-2020
        public static int MIN_SHOTS = 0;//123-295
        public static int MAX_SHOTS = 60;
        public static int MIN_KICKS = 120;//123-295
        public static int MAX_KICKS = 300;
        public static int MIN_HANDBALLS = 50;//58-297
        public static int MAX_HANDBALLS = 300;
        public static int MIN_MARKS = 20;//21-181
        public static int MAX_MARKS = 200;
        public static int MIN_HITOUTS = 0;//4-89
        public static int MAX_HITOUTS = 100;
        public static int MIN_TACKLES = 10;//17-155
        public static int MAX_TACKLES = 160;
        public static int MIN_FREES = 0;//4-38
        public static int MAX_FREES = 40;
        public static int MIN_RUSHEDBEHINDS = 0;//0-11
        public static int MAX_RUSHEDBEHINDS = 15;

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
        public int RushedBehinds { get; set; }
        //2010+
        public int Clearances { get; set; }
        public int Clangers { get; set; }
        public int Rebound50s { get; set; }
        public int Inside50s { get; set; }
    }
}
