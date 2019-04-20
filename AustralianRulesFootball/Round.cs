using System.Collections.Generic;

namespace AustralianRulesFootball
{
    public class Round : RoundShell
    {
        public override int GetHashCode()
        {
            return (Matches != null ? Matches.GetHashCode() : 0);
        }

        public List<Match> Matches;
        public bool IsFinal { get; set; }

        public Round() : this(0, 0, false, new List<Match>()) { }

        public Round(int year, int number, List<Match> matches) : this(year, number, false, matches) { }

        public Round(int year, int number, bool isFinal, List<Match> matches)
        {
            Matches = matches;
            Number = number;
            Year = year;
            IsFinal = isFinal;
        }

        public override bool Equals(object other)
        {
            return Equals(other as Round);
        }

        public bool Equals(Round other)
        {
            if (other == null)
                return false;
            return Year.Equals(other.Year)
                && Number.Equals(other.Number);
        }

        #region IO
        /*public string Stringify()
        {
            var s = "";
            s += "<year>" + Year + "</year>";
            s += "<number>" + Number + "</number>";
            s += "<matches>";
            foreach (var m in Matches)
            {
                s += "<match>";
                s += m.Stringify();
                s += "</match>";
            }
            s += "</matches>";
            return s;
        }

        public BsonDocument Bsonify()
        {
            var matches = new BsonArray();
            foreach (var match in Matches)
            {
                matches.Add(match.Bsonify());
            }

            var round = new BsonDocument
            {
                {"matches", matches},
                {"number", Number},
                {"year", Year}
            };

            return round;
        }

        public static Round Objectify(string str)
        {
            var year = Convert.ToInt32(Stringy.SplitOn(str, "year")[0]);
            var number = Convert.ToInt32(Stringy.SplitOn(str, "number")[0]);
            var ms = Stringy.SplitOn(Stringy.SplitOn(str, "matches")[0], "match");

            if (ms.Count == 0 || ms[0] == "0" || ms[0] == "")
                return new Round();

            var matches = ms.Select(Match.Objectify).ToList();
            return new Round(year, number, matches);
        }

        public static Round Objectify(BsonDocument bson)
        {
            var year = bson.GetValue("year").AsInt32;
            var number = bson.GetValue("number").AsInt32;
            var matches = bson.GetValue("matches").AsBsonArray.Select(r => Match.Objectify(r.AsBsonDocument)).ToList();

            return new Round(year, number, matches);
        }*/
        #endregion
    }
}
