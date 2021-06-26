using System;
using MongoDB.Bson;
using Utilities;

namespace AustralianRulesFootball
{
    public class Score : Entity
    {
        public double Goals;
        public double Points;
        public bool Err;

        public Score():this(0, 0){}

        public Score(string score)
        {
            try
            {
                var g = score.Substring(0, score.IndexOf('.'));
                var p = score.Substring(score.IndexOf('.') + 1, score.Length - (score.IndexOf('.') + 1));
                Goals = Convert.ToInt32(g);
                Points = Convert.ToInt32(p);
            }
            catch(Exception)
            {
                Goals = 0;
                Points = 0;
                Err = true;
            }
            
        }

        public Score(double goals, double points)
        {
            Goals = goals;
            Points = points;
        }

        public double Total()
        {
            return (6 * Goals) + Points;
        }
        public double Shots()
        {
            return Goals + Points;
        }

        public static Score operator +(Score s1, Score s2)
        {
            return new Score(s1.Goals + s2.Goals, s1.Points + s2.Points);
        }

        public static Score operator -(Score s1, Score s2)
        {
            return new Score(s1.Goals - s2.Goals, s1.Points - s2.Points);
        }

        #region IO
        public string Stringify()
        {
            var s = "";
            s += "<goals>" + Goals + "</goals>";
            s += "<points>" + Points + "</points>";
            return s;
        }

        public BsonValue Bsonify()
        {
            var quarter = new BsonDocument
            {
                {"goals", Goals},
                {"points", Points},
            };
            return quarter;
        }

        public static Score Objectify(string str)
        {
            var goals = Convert.ToInt32(Stringy.SplitOn(str, "goals")[0]);
            var points = Convert.ToInt32(Stringy.SplitOn(str, "points")[0]);

            return new Score(goals, points);
        }

        public static Score Objectify(BsonDocument bson)
        {
            var goals = bson.GetValue("goals").AsDouble;
            var points = bson.GetValue("points").AsDouble;
            return new Score(goals, points);
        }
        #endregion
    }
}
