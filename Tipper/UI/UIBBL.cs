using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tipper.UI
{
    public class UIBBL
    {
        public static void LoadMainLoop()
        {
            var loop = true;
            Console.WriteLine("BBL Tipper");
            while (loop)
            {
                Console.Write(">");
                var command = Console.ReadLine();
                if (command == null) continue;
                switch (command.ToUpper())
                {
                    case ("F"):
                        TipFullSeason();
                        break;
                    case ("Q"):
                        loop = false;
                        break;
                    case ("T"):
                        TwitterTipNextRound();
                        break;
                    case ("Z"):
                        Testing();
                        break;
                    case ("?"):
                        ListOptions();
                        break;
                }
            }

        }

        public static void ListOptions()
        {
            Console.WriteLine("Tip [F]ull season");
            Console.WriteLine("[Q]uit");
            Console.WriteLine("[T]witter tipping");
            Console.WriteLine("[Z] Testing");
            Console.WriteLine("[?] Show options");
        }

        private static void TipFullSeason()
        {
            throw new NotImplementedException();
        }

        private static void TwitterTipNextRound()
        {
            throw new NotImplementedException();
        }

        private static void Testing()
        {
            throw new NotImplementedException();
        }
    }
}
