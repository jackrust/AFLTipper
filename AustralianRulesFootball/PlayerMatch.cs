using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace AustralianRulesFootball
{
    public class PlayerMatch
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
            throw new NotImplementedException();
        }

        public static PlayerMatch Objectify(BsonDocument bson)
        {
            throw new NotImplementedException();
        }
    }
}
