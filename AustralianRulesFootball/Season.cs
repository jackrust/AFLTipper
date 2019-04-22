using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Utilities;

namespace AustralianRulesFootball
{
    public class Season : Entity
    {
        public int Year;
        public List<Round> Rounds;

        public Season(int year, List<Round> rounds)
        {
            Year = year;
            Rounds = rounds;
        }

        public List<Round> GetRounds(int fromRound, int toRound)
        {
            if (fromRound < 0)
                fromRound = Rounds.Count - fromRound;
            if (toRound < 0)
                toRound = Rounds.Count - toRound;
            if (fromRound < 0)
                fromRound = 0;
            if (toRound < 0)
                toRound = 0;

            return Rounds.Where(r => r.Number >= fromRound && r.Number <= toRound).ToList();
        }

        public List<Round> GetRoundsFrom(int fromRound)
        {
            return GetRounds(fromRound, Rounds.Count);
        }

        public List<Round> GetRoundsTo(int toRound)
        {
            return GetRounds(0, toRound);
        }

        public List<Match> GetMatches(int fromRound, int toRound)
        {
            var rounds = GetRounds(fromRound, toRound);
            var matches = new List<Match>();
            foreach (var r in rounds)
            {
                matches.AddRange(r.Matches);
            }
            return matches;
        }

        public List<Match> GetMatches()
        {
            var matches = new List<Match>();
            foreach (var r in Rounds)
            {
                matches.AddRange(r.Matches);
            }
            return matches;
        }

        public List<Match> GetMatches(Team team)
        {
            var matches = new List<Match>();
            foreach (var r in Rounds)
            {
                matches.AddRange(r.Matches.Where(m => m.Home.ApiName == team.ApiName || m.Away.ApiName == team.ApiName));
            }
            return matches;
        }

        #region IO
        public string Stringify()
        {
            var s = "";
            s += "<year>"+Year+"</year>";
            s += "<id>"+Id+"</id>";
            s += "<rounds>";
            foreach (var r in Rounds)
            {
                s += "<round>";
                s += r.Stringify();
                s += "</round>";
            }
            s += "</rounds>";
            return s;
        }
        /*
        public BsonDocument Bsonify()
        {
            //TODO: this probably shouldn't know about Bson
            var rounds = new BsonArray();
            foreach (var round in Rounds)
            {
                rounds.Add(round.Bsonify());
            }

            var match = new BsonDocument
            {
                {"year", Year},
                {"rounds", rounds}
            };
            return match;
        }*/

        public static Season Objectify(string str)
        {
            var year = Convert.ToInt32(Stringy.SplitOn(str, "year")[0]);
            var rs = Stringy.SplitOn(Stringy.SplitOn(str, "rounds")[0], "round");
            var rounds = rs.Select(Round.Objectify).ToList();
            var season = new Season(year, rounds);
            return season;
        }
        /*
        public static Season Objectify(BsonDocument bson)
        {
            var year = bson.GetValue("year").AsInt32;
            var rounds = bson.GetValue("rounds").AsBsonArray.Select(r => Round.Objectify(r.AsBsonDocument)).ToList();

            var season =  new Season(year, rounds);
            return season;
        }*/
        #endregion
    }
}
