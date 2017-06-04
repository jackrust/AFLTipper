using System;
using MongoDB.Bson;
using Utilities;

namespace AustralianRulesFootball
{
    public class Quarter
    {
        public enum QuarterNumber
        {
            One = 1,
            Two = 2,
            Three = 3,
            Four = 4
        }
        public QuarterNumber Number;
        public Score HomeScore;
        public Score AwayScore;

        public Quarter() : this(0, new Score(), new Score())
        {
        }

        public Quarter(int number, Score homeScore, Score awayScore)
        {
            Number = (QuarterNumber)number;
            HomeScore = homeScore;
            AwayScore = awayScore;
        }

        #region IO
        public string Stringify()
        {
            var s = "";
            s += "<number>" + (int)Number + "</number>";
            s += "<homescore>" + HomeScore.Stringify() + "</homescore>";
            s += "<awayscore>" + AwayScore.Stringify() + "</awayscore>";
            return s;
        }

        public BsonValue Bsonify()
        {
            var quarter = new BsonDocument
            {
                {"number", Number},
                {"homeScore", HomeScore.Bsonify()},
                {"awayScore", AwayScore.Bsonify()},
            };
            return quarter;
        }

        public static Quarter Objectify(string str)
        {
            var number = Convert.ToInt32(Stringy.SplitOn(str, "number")[0]);
            var homeScore = Score.Objectify(Stringy.SplitOn(str, "homescore")[0]);
            var awayScore = Score.Objectify(Stringy.SplitOn(str, "awayscore")[0]);
            return new Quarter(number, homeScore, awayScore);
        }

        public static Quarter Objectify(BsonDocument bson)
        {
            var number = bson.GetValue("number").AsInt32;
            var homeScore = Score.Objectify(bson.GetValue("homeScore").AsBsonDocument);
            var awayScore = Score.Objectify(bson.GetValue("awayScore").AsBsonDocument);
            return new Quarter(number, homeScore, awayScore);
        }
        #endregion
    }
}
