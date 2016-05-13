using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.WireProtocol.Messages;

namespace AFLStatisticsService
{
    class Program
    {
        static void Main(string[] args)
        {
            var db = new MongoDB();
            //db.InsertDocument();
            Console.ReadLine();
            db.ReadDocument();
            Console.ReadLine();
        }

        
    }
}
