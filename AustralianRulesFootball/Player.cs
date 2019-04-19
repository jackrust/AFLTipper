using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;

namespace AustralianRulesFootball
{
    public class Player
    {
        public int FinalSirenPlayerId { get; set; }
        public string Name { get; set; }
        public List<PlayerMatch> History { get; set; }

        public BsonDocument Bsonify()
        {
            var history = new BsonArray();
            foreach (var playerMatch in History)
            {
                history.Add(playerMatch.Bsonify());
            }

            var player = new BsonDocument
            {
                {"finalSirenPlayerId", FinalSirenPlayerId},
                {"name", Name},
                {"history", history}
            };

            return player;
        }

        public static Player Objectify(BsonDocument bson)
        {
            var finalSirenPlayerId = bson.GetValue("finalSirenPlayerId").AsInt32;
            var name = bson.GetValue("name").AsString;
            var history = bson.GetValue("history").AsBsonArray.Select(r => PlayerMatch.Objectify(r.AsBsonDocument)).ToList();

            return new Player()
                {
                    FinalSirenPlayerId = finalSirenPlayerId,
                    Name = name,
                    History = history
                };
        }
    }
}
