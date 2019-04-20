using MongoDB.Bson;

namespace AustralianRulesFootball
{
    public class PlayerMatch : Entity
    {
        public int FinalSirenPlayerId { get; set; }
        public int Year { get; set; }
        public int RoundNo { get; set; }
        public string Against { get; set; }
        public int Kicks { get; set; }
        public int Handballs { get; set; }
        public int Marks { get; set; }
        public int HitOuts { get; set; }
        public int Tackles { get; set; }
        public int FreesFor { get; set; }
        public int FreesAgainst { get; set; }
        public int Goals { get; set; }
        public int Behinds { get; set; }
        public int Rating { get; set; }
        public bool Win { get; set; }

        public BsonDocument Bsonify()
        {
            var playerMatch = new BsonDocument
            {
                {"finalSirenPlayerId", FinalSirenPlayerId},
                {"year", Year},
                {"roundNo", RoundNo},
                {"against", Against},
                {"kicks", Kicks},
                {"handballs", Handballs},
                {"marks", Marks},
                {"hitOuts", HitOuts},
                {"tackles", Tackles},
                {"freesFor", FreesFor},
                {"freesAgainst", FreesAgainst},
                {"goals", Goals},
                {"behinds", Behinds},
                {"rating", Rating},
                {"win", Win},
            };

            return playerMatch;
        }

        public static PlayerMatch Objectify(BsonDocument bson)
        {
            var finalSirenPlayerId = bson.GetValue("finalSirenPlayerId").AsInt32;
            var year = bson.GetValue("year").AsInt32;
            var roundNo = bson.GetValue("roundNo").AsInt32;
            var against = bson.GetValue("against").AsString;
            var kicks = bson.GetValue("kicks").AsInt32;
            var handballs = bson.GetValue("handballs").AsInt32;
            var marks = bson.GetValue("marks").AsInt32;
            var hitOuts = bson.GetValue("hitOuts").AsInt32;
            var tackles = bson.GetValue("tackles").AsInt32;
            var freesFor = bson.GetValue("freesFor").AsInt32;
            var freesAgainst = bson.GetValue("freesAgainst").AsInt32;
            var goals = bson.GetValue("goals").AsInt32;
            var behinds = bson.GetValue("behinds").AsInt32;
            var rating = bson.GetValue("rating").AsInt32;
            var win = bson.GetValue("win").AsBoolean;

            return new PlayerMatch()
            {
                FinalSirenPlayerId = finalSirenPlayerId,
                Year = year,
                RoundNo = roundNo,
                Against = against,
                Kicks = kicks,
                Handballs = handballs,
                Marks = marks,
                HitOuts = hitOuts,
                Tackles = tackles,
                FreesFor = freesFor,
                FreesAgainst = freesAgainst,
                Goals = goals,
                Behinds = behinds,
                Rating = rating,
                Win = win
            };
        }
    }
}
