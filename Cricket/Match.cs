using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

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
        public bool Abandoned = false;
        public MatchResult Result;
        //public List<Inning> Innings;

        public Match()
        {
            HomeScore = new MatchScore();
            AwayScore = new MatchScore();
            Result = new MatchResult();
        }

        public int EffectiveID()
        {
            return (Date.Year * 100) + Number;
        }

        public bool Tie()
        {
            return HomeScore.Runs == AwayScore.Runs;
        }



        public bool WinFor(Team team)
        {
            return team.Equals(Home) ? HomeWin() : team.Equals(Away) ? AwayWin() : false;
        }

        public MatchScore ScoreFor(Team team)
        {
            return team.Equals(Home) ? HomeScore : team.Equals(Away) ? AwayScore : new MatchScore();
        }

        public MatchScore ScoreAgainst(Team team)
        {
            return team.Equals(Home) ? AwayScore : team.Equals(Away) ? HomeScore : new MatchScore();
        }

        public double WinningnessFor(Team team)
        {
            return team.Equals(Home) ? HomeWinningness() : team.Equals(Away) ? AwayWinningness() : 0.5;
        }

        public bool HomeWin()
        {
            return Result.Victor == Victor.Home;
        }

        public bool AwayWin()
        {
            return Result.Victor == Victor.Away;
        }

        public double HomeWinningness()
        {
            const double MaxWickets = 10.0;
            const double MaxRuns = 75.0;
            var margin = 0.5;
            var marginAddition = 0.0;

            if (Result.MarginByWickets > 0)
            {
                marginAddition = Numbery.Normalise(Result.MarginByWickets, 0, MaxWickets, 0, 0.5);
            } else if (Result.MarginByRuns > 0) 
            {
                marginAddition = Numbery.Normalise(Result.MarginByRuns, 0, MaxRuns, 0, 0.5);
            }

            if (Result.Victor == Victor.Home)
                margin += marginAddition;
            else if (Result.Victor == Victor.Away)
                margin -= marginAddition;

            return margin;
        }

        public double AwayWinningness()
        {
            const double MaxWickets = 10.0;
            const double MaxRuns = 75.0;
            var margin = 0.5;
            var marginAddition = 0.0;

            if (Result.MarginByWickets > 0)
            {
                marginAddition = Numbery.Normalise(Result.MarginByWickets, 0, MaxWickets, 0, 0.5);
            }
            else if (Result.MarginByRuns > 0)
            {
                marginAddition = Numbery.Normalise(Result.MarginByRuns, 0, MaxRuns, 0, 0.5);
            }

            if (Result.Victor == Victor.Away)
                margin += marginAddition;
            else if (Result.Victor == Victor.Home)
                margin -= marginAddition;

            return margin;
        }

        public bool HasTeam(Team team)
        {
            return Away.Id == team.Id || Home.Id == team.Id;
        }
    }

    public class MatchScore
    {
        public OverValue Overs = new OverValue();
        public int Runs;
        public int Wickets;

        public MatchScore()
        {
            Overs = new OverValue();
        }

        public double Average()
        {
            //TODO: Not a true average :( need to avoid infinity
            return Runs / (Wickets+1.0);
        }

        public double StrikeRate()
        {
            if (Overs == null)
                return 0;
            if (Overs.Overs == 0)
                return 0;
            return Runs / ((Overs.Overs * 6.0 + Overs.Balls) / 6.0);
        }

        public static MatchScore Str2ScoreEnglish(string str)
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
            else
                score.Wickets = 10;

            if(scoreParts.Length > 2)
                o.Overs = Int32.Parse(scoreParts[2]);
            if (scoreParts.Length > 3)
                o.Balls = Int32.Parse(scoreParts[3]);
            score.Overs = o;
            return score;
        }

        public static MatchScore Str2ScoreAustralian(string str)
        {
            var tokens = new char[] { '/', '(', '.' };
            var score = new MatchScore();
            var o = new OverValue();
            if (!str.Contains("/"))
                str = "10/" + str;
            //7/156 (20 overs)
            var scoreParts = str.Replace(")", "").Split(tokens);

            if (scoreParts.Length > 0)
                score.Wickets = Int32.Parse(scoreParts[0]);
            if (scoreParts.Length > 1)
                score.Runs = Int32.Parse(scoreParts[1]);

            if (scoreParts.Length > 2)
                o.Overs = Int32.Parse(scoreParts[2].Replace("overs", "").Trim());
            if (scoreParts.Length > 3)
                o.Balls = Int32.Parse(scoreParts[3].Replace("overs", "").Trim());
            score.Overs = o;
            return score;
        }
    }
}
