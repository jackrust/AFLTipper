using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace AustralianRulesFootball
{
    public class Util : Entity
    {
        public static double MaxScore = 250; //250- Ceiling score scored in an AFL Game - semi static
        public static double MaxAverage = 180; //Ceiling score scored in an AFL Game - semi static
        public static double MaxGoals = 40; //Ceiling goals scored in an AFL Game - semi static
        public static double MaxPoints = 40; //Ceiling points scored in an AFL Game - semi static
        public static double MaxMargin = 119; //119 - Ceiling margin scored in an AFL Game - semi static
        public static double MaxRestDays = 21; //estimate maximum inseason days between games - semi static
        public static double MaxLadderPoints = 4;

        public static List<Team> GetTeams()
        {
            var teams = new List<Team>
            {
                Team.Adelaide,
                Team.Brisbane,
                Team.Carlton,
                Team.Collingwood,
                Team.Essendon,
                Team.Fremantle,
                Team.Geelong,
                Team.GoldCoast,
                Team.GreaterWesternSydney,
                Team.Hawthorn,
                Team.Melbourne,
                Team.NorthMelbourne,
                Team.PortAdelaide,
                Team.Richmond,
                Team.StKilda,
                Team.Sydney,
                Team.WestCoast,
                Team.Western
            };
            return teams;
        }

        public static List<Ground> GetGrounds()
        {
            var grounds = new List<Ground>
            {
                Ground.A,
                Ground.Ao,
                Ground.P,
                Ground.Anz,
                Ground.As,
                Ground.Au,
                Ground.Bisp,
                Ground.Ba,
                Ground.Bs,
                Ground.Cs,
                Ground.Es,
                Ground.Eu,
                Ground.G,
                Ground.Jo,
                Ground.Jss,
                Ground.Lo,
                Ground.Lho,
                Ground.Mclp,
                Ground.Mcg,
                Ground.Ms,
                Ground.Nh,
                Ground.Os,
                Ground.Ps,
                Ground.Rs,
                Ground.Ss,
                Ground.Sss,
                Ground.Sps,
                Ground.Sto,
                Ground.Scg,
                Ground.Tio,
                Ground.Ttp,
                Ground.V,
                Ground.Waca,
                Ground.Ws,
                Ground.Wo,
                Ground.Wh
            };
            return grounds;
        }

        public static Team GetTeamByName(String name)
        {

            Team team = GetTeams()
                        .First();
            try
            {
                team =
                    GetTeams()
                        .First(
                            t => t.Names.Any(a => String.Equals(a, name, StringComparison.CurrentCultureIgnoreCase)));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return team;
        }

        public static Ground GetGroundByName(string name)
        {
            Ground ground = GetGrounds()
                        .First();
            try
            {
                ground =
                    GetGrounds()
                        .First(
                            g =>
                                g.Names.Any(a => String.Equals(a, name, StringComparison.CurrentCultureIgnoreCase)) ||
                                String.Equals(g.Abbreviation, name, StringComparison.CurrentCultureIgnoreCase));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return ground;
        }

        public static DateTime StringToDate(string datetime)
        {
            //"Mar 30 (Fri 7:40pm) 2007"
            string[] formats = { " dd/MMM/yyyy h:mm tt", " d /MMM/yyyy h:mm tt", " d /MMM/yyyy  ", " dd/MMM/yyyy  ", 
                                   "dddd, MMMM dd h:mmtt yyyy", "MMM d (ddd h:mmtt) yyyy", "MMM d (ddd) yyyy" };
            /*DateTime date;
            var remainder = datetime;
            var month = remainder.Substring(0, 3);
            remainder = remainder.Substring(3, remainder.Length - 3);
            var day = remainder.Substring(0, 3);
            remainder = remainder.Substring(remainder.IndexOf("(", StringComparison.Ordinal) + 1,
                remainder.IndexOf(")", StringComparison.Ordinal) -
                (remainder.IndexOf("(", StringComparison.Ordinal) + 1));
            var ampm = "";
            var time = "";
            if (remainder.IndexOf(":", StringComparison.Ordinal) > 0)
            {
                ampm = remainder.Substring(remainder.Length - 2, 2);
                remainder = remainder.Substring(4, remainder.Length - 6);
                time = remainder;
            }

            var dateString = day + "/" + month.ToUpper() + "/" + year + " " + time + " " + ampm.ToUpper();*/
            DateTime date;
            var dateString = datetime;
            if (DateTime.TryParseExact(dateString, formats,
                new CultureInfo("en-US"),
                DateTimeStyles.None,
                out date))
                return date;
            return new DateTime();
        }

        public static string ConvertToDoubleDigit(string orginal)
        {
            return Regex.Replace(orginal, @"\d", "");
        }
    }
}
