using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cricket
{
    public enum Format
    {
        Twenty20,
        OneDay,
        TestMatch
    }

    public class Match
    {
        //public Format Format;
        public int Number;
        public Team Home;
        public Team Away;
        public AustralianRulesFootball.Ground Ground;
        public DateTime Date;
        public MatchScore HomeScore;
        public MatchScore AwayScore;
        //public List<Inning> Innings;
    }

    public class MatchScore
    {
        public OverValue Overs;
        public int Runs;
        public int Wickets;

        public static MatchScore Str2Score(string str)
        {
            var tokens = new char[] {'/','(', '.'};
            var score = new MatchScore();
            var o = new OverValue();
            //176/3(18.4)
            var scoreParts = str.Replace(")", "").Split(tokens);

            if (scoreParts.Length > 0)
                score.Runs = Int32.Parse(scoreParts[0]);
            if (scoreParts.Length > 1)
                score.Wickets = Int32.Parse(scoreParts[1]);
            if(scoreParts.Length > 2)
                o.Overs = Int32.Parse(scoreParts[2]);
            if (scoreParts.Length > 3)
                o.Balls = Int32.Parse(scoreParts[3]);
            score.Overs = o;
            return score;
        }
    }
}
