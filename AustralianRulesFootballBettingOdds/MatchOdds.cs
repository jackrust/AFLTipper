using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AustralianRulesFootball;
using Utilities;

namespace AustralianRulesFootballBettingOdds
{
    public class MatchOdds
    {
        public DateTime Date;
        public Team Home;
        public Team Away;
        public Ground Venue;
        public Score HomeScore;
        public Score AwayScore;
        public bool IsFinal;
        public double HomeOdds;
        public double AwayOdds;

        public static MatchOdds CreateFromCsv(string csv)
        {
            var columns = csv.Split(',');
            var matchOdds = new MatchOdds();
            //Date
            var dateString = columns[0] + " " + columns[1];
            DateTime dt;
            DateTime.TryParseExact(dateString, "dd-MMM-yy HH:mm", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out dt);
            matchOdds.Date = dt;
            //Teams
            matchOdds.Home = Util.GetTeamByName(columns[2]);
            matchOdds.Away = Util.GetTeamByName(columns[3]);
            //Ground
            matchOdds.Venue = Util.GetGroundByName(columns[4]);
            //IsFinal
            matchOdds.IsFinal = columns[7].Contains("Y");
            //Scores
            double homeGoals;
            double homePoints;
            double awayGoals;
            double awayPoints;
            Double.TryParse(columns[8], out homeGoals);
            Double.TryParse(columns[9], out homePoints);
            Double.TryParse(columns[10], out awayGoals);
            Double.TryParse(columns[11], out awayPoints);
            matchOdds.HomeScore = new Score(homeGoals, homePoints);
            matchOdds.AwayScore = new Score(awayGoals, awayPoints);
            //Odds
            double homeOdds;
            double awayOdds;
            Double.TryParse(columns[12], out homeOdds);
            Double.TryParse(columns[13], out awayOdds);
            matchOdds.HomeOdds = homeOdds;
            matchOdds.AwayOdds = awayOdds;
            return matchOdds;
        }

        public static List<MatchOdds> LoadMatchOddsList()
        {
            var rows = Filey.LoadLines("HistoricalOdds.csv");
            var oddsList = rows.Skip(1).Select(CreateFromCsv).ToList();
            return oddsList;
        } 
    }
}
