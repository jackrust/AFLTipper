using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using Utilities;

namespace AustralianRulesFootball
{
    public class Match
    {
        public Team Home;
        public Team Away;
        public Ground Ground;
        public DateTime Date;
        public List<Quarter> Quarters;
        public MatchStatistics HomeStats { get; set; }
        public MatchStatistics AwayStats { get; set; }
        public List<PlayerMatch> HomePlayerMatches { get; set; }
        public List<PlayerMatch> AwayPlayerMatches { get; set; }

        public double HomeOdds;
        public double AwayOdds;

        public Match(Team home, Score hq1, Score hq2, Score hq3, Score hq4, Team away, Score aq1, Score aq2, Score aq3,
            Score aq4, Ground ground, DateTime date)
            : this(home, away,
                new List<Quarter>()
                {
                    new Quarter(1, hq1, aq1),
                    new Quarter(2, hq2, aq2),
                    new Quarter(3, hq3, aq3),
                    new Quarter(4, hq4, aq4)
                },
                ground, date, 0.0, 0.0){}

        public Match(Team home, Team away, List<Quarter> quarters, Ground ground, DateTime date, double homeOdds, double awayOdds)
        {
            Home = home;
            Away = away;
            Ground = ground;
            Date = date;
            Quarters = quarters;
            HomeOdds = homeOdds;
            AwayOdds = awayOdds;
            //TODO: should these always be set?
            HomePlayerMatches = new List<PlayerMatch>();
            AwayPlayerMatches = new List<PlayerMatch>();
        }

        

        public Score ScoreFor(Team team)
        {
            return team.Equals(Home) ? HomeScore() : team.Equals(Away) ? AwayScore() : new Score();
        }

        public Score ScoreFor(List<Team> teams)
        {
            foreach (var t in teams.Where(HasTeam))
            {
                return ScoreFor(t);
            }
            return new Score();
        }

        public Score ScoreAgainst(Team team)
        {
            return team.Equals(Home) ? AwayScore() : team.Equals(Away) ? HomeScore() : new Score();
        }

        public Score ScoreAgainst(List<Team> teams)
        {
            foreach (var t in teams.Where(HasTeam))
            {
                return ScoreAgainst(t);
            }
            return new Score();
        }

        public Score HomeScore()
        {
            var score = new Score();
            return Quarters.Aggregate(score, (current, q) => current + q.HomeScore);
        }

        public Score AwayScore()
        {
            var score = new Score();
            return Quarters.Aggregate(score, (current, q) => current + q.AwayScore);
        }

        public double Margin()
        {
            return Math.Abs(HomeScore().Total() - AwayScore().Total());
        }

        public double TotalScore()
        {
            return HomeScore().Total() + AwayScore().Total();
        }

        public bool IsWinningTeam(Team team)
        {
            if (!team.Equals(Home) && !team.Equals(Away))
                return false;
            return ScoreFor(team).Total() > ScoreAgainst(team).Total();
        }

        public Team GetLosingTeam()
        {
            return ScoreFor(Home).Total() > ScoreFor(Away).Total() ? Home : Away;
        }

        public double HomeLadderPoints()
        {
            return HomeScore().Total() > AwayScore().Total() ? 4 : Math.Abs(HomeScore().Total() - AwayScore().Total()) < 0.5 ? 2 : 0;
        }

        public double AwayLadderPoints()
        {
            return AwayScore().Total() > HomeScore().Total() ? 4 : Math.Abs(AwayScore().Total() - HomeScore().Total()) < 0.5 ? 2 : 0;
        }

        public bool HasTeam(Team team)
        {
            return Away.Equals(team) || Home.Equals(team);
        }

        public bool HasTeam(List<Team> teams)
        {
            return teams.Aggregate(false, (current, t) => HasTeam(t) || current);
        }

        public Team GetOpposition(Team team)
        {
            return Home.Equals(team) ? Away : Home;
        }

        public Team GetOpposition(List<Team> teams)
        {
            if (teams.Count(t => t.Equals(Home) || t.Equals(Away)) == 0) return null;
            if (teams.Count(t => t.Equals(Home)) > 0 && teams.Count(t => t.Equals(Away)) > 0) return null;
            if (teams.Count(t => t.Equals(Home)) > 0) return Away;
            if (teams.Count(t => t.Equals(Away)) > 0) return Home;
            return null;
        }

        public Score GetOppositionScore(Team team)
        {
            if (Home.Equals(team))
            {
                return AwayScore();
            }
            if (Away.Equals(team))
            {
                return HomeScore();
            }
            return new Score();
        }

        public Score GetTeamScore(Team team)
        {
            if (Home.Equals(team))
            {
                return HomeScore();
            }
            if (Away.Equals(team))
            {
                return AwayScore();
            }
            return new Score();
        }

        public bool Equals(Match other)
        {
            return Date.Equals(other.Date) && Home.Equals(other.Home) && Away.Equals(other.Away) &&
                   Ground.Equals(other.Ground);
        }

        #region IO

        public string Stringify()
        {
            var s = "";
            s += "<hometeam>" + Home.Stringify() + "</hometeam>";
            s += "<awayteam>" + Away.Stringify() + "</awayteam>";
            s += "<ground>" + Ground.Stringify() + "</ground>";
            s += "<date>" + Date + "</date>";
            s += "<quarters>";
            foreach (var q in Quarters)
            {
                s += "<quarter>";
                s += q.Stringify();
                s += "</quarter>";
            }
            s += "</quarters>";
            s += "<homeOdds>" + HomeOdds + "</homeOdds>";
            s += "<awayOdds>" + AwayOdds + "</awayOdds>";
            return s;
        }

        public BsonDocument Bsonify()
        {
            //TODO: this probably shouldn't know about Bson
            var quarters = new BsonDocument()
            {
                {"1", Quarters[0].Bsonify()},
                {"2", Quarters[1].Bsonify()},
                {"3", Quarters[2].Bsonify()},
                {"4", Quarters[3].Bsonify()}
            };

            var match = new BsonDocument
            {
                {"home", Home.ApiName},
                {"away", Away.ApiName},
                {"ground", Ground.Abbreviation},
                {"date", Date},
                {"quarters", quarters},
                {"homeOdds", HomeOdds},
                {"awayOdds", AwayOdds}
            };
            return match;
        }

        public static Match Objectify(string str)
        {
            var home = Team.Objectify(Stringy.SplitOn(str, "hometeam")[0]);
            var away = Team.Objectify(Stringy.SplitOn(str, "awayteam")[0]);
            var ground = Ground.Objectify(Stringy.SplitOn(str, "ground")[0]);
            var d = Stringy.SplitOn(str, "date")[0];
            var date = new DateTime();
            if (d.Length > 1)
                date = DateTime.Parse(d);
            var qs = Stringy.SplitOn(Stringy.SplitOn(str, "quarters")[0], "quarter");
            var quarters = qs.Select(Quarter.Objectify).ToList();
            var hos = Stringy.SplitOn(str, "homeOdds")[0];
            var homeOdds = Double.Parse(hos);
            var aos = Stringy.SplitOn(str, "awayOdds")[0];
            var awayOdds = Double.Parse(aos);
            return new Match(home, away, quarters, ground, date, homeOdds, awayOdds);
        }

        public static Match Objectify(BsonDocument bson)
        {
            var home = Team.LoadByName(bson.GetValue("home").AsString);
            var away = Team.LoadByName(bson.GetValue("away").AsString);
            var ground = Ground.LoadByAbbreviation(bson.GetValue("ground").AsString);
            var date = bson.GetValue("date").ToUniversalTime();
            var qs = bson.GetValue("quarters").AsBsonDocument;
            var q1 = Quarter.Objectify(qs.GetValue("1").AsBsonDocument);
            var q2 = Quarter.Objectify(qs.GetValue("2").AsBsonDocument);
            var q3 = Quarter.Objectify(qs.GetValue("3").AsBsonDocument);
            var q4 = Quarter.Objectify(qs.GetValue("4").AsBsonDocument);
            var quarters = new List<Quarter> {q1, q2, q3, q4};
            var homeOdds = bson.GetValue("homeOdds").AsDouble;
            var awayOdds = bson.GetValue("awayOdds").AsDouble;
            return new Match(home, away, quarters, ground, date, homeOdds, awayOdds);
        }
        #endregion
    }
}
