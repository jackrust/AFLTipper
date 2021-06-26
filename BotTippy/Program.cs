using AFLStatisticsService;
using System;
using Tipper.UI;
using System.Configuration;

namespace BotTippy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hi!");
            Console.WriteLine("Let me just check what's happened since last week...");
            StatisticsServiceUI.UpdateMatchesFootyWire();

            Console.WriteLine("All caught up, time to tip!");
            UIMainLoop.TwitterTipNextRound();

            Console.WriteLine("Tipped!");
            Console.ReadLine();
        }
    }
}