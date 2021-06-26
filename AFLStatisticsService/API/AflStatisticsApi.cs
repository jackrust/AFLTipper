using System;
using System.Collections.Generic;
using System.Linq;
using AustralianRulesFootball;

namespace AFLStatisticsService.API
{
    public abstract class AflStatisticsApi
    {
        protected static readonly Dictionary<int, int> numHomeandAwayRounds = new Dictionary<int, int>
        {
            {2021, 23},
            {2020, 18},
            {2019, 23},
            {2018, 23},
            {2017, 23},    
            {2016, 23},
            {2015, 23},
            {2014, 23},
            {2013, 23},
            {2012, 23},
            {2011, 24},
            {2010, 22},//5
            {2009, 22},
            {2008, 22},
            {2007, 22},
            {2006, 22},
            {2005, 22},
            {2004, 22},
            {2003, 22},
            {2002, 22},
            {2001, 22},
            {2000, 22},
            {1999, 22},
            {1998, 22},
            {1997, 22},
            {1996, 22},
            {1995, 22},
            {1994, 24},
            {1993, 22},
            {1992, 24},
            {1991, 24},
        };

        public List<Season> UpdateFrom(List<Season> seasons, RoundUid roundUid)
        {
            if (seasons.Count == 1)
                seasons = new List<Season>();
            var number = roundUid.Number;
            var year = roundUid.Year;
            if (roundUid.IsFinal)
                number = numHomeandAwayRounds[year];

            var successful = true;
            while (successful)
            {
                try
                {
                    if (seasons.All(s => s.Year != year))
                    {
                        seasons.Add(new Season(year, new List<Round>()));
                    }

                    var numRounds = GetNumRounds(year);

                    if(numRounds == 0)
                    {
                        successful = false;
                    }
                    for (var i = number; i <= numRounds; i++)
                    {
                        var round = GetRoundResultsHomeAndAway(year, i);
                        if (seasons.First(s => s.Year == year).Rounds.Count >= i)
                        {
                            seasons.First(s => s.Year == year).Rounds[i - 1] = round;
                        }
                        else
                        {
                            seasons.First(s => s.Year == year).Rounds.Add(round);
                        }
                    }

                    var finals = GetRoundResultsFinals(year);
                    var finalNumber = 0;
                    foreach (var r in finals)
                    {
                        finalNumber++;
                        if (seasons.First(s => s.Year == year).Rounds.Count >= (numRounds+finalNumber))
                        {
                            seasons.First(s => s.Year == year).Rounds[(numRounds + finalNumber) - 1] = r;
                        }
                        else
                        {
                            seasons.First(s => s.Year == year).Rounds.Add(r);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine(e.StackTrace);
                    successful = false;
                }
                if (successful)
                {
                    //We're still getting fresh data so loop into next season:
                    year++;
                    number = 1;
                    seasons.Add(new Season(year, new List<Round>()));
                }
            }
            return seasons;
        }

        //public abstract List<Season> UpdateFrom(List<Season> seasons, int year, int number);

        public abstract int GetNumRounds(int year);

        public abstract Round GetRoundResultsHomeAndAway(int year, int roundNo);
        public abstract List<Round> GetRoundResultsFinals(int year);

        public abstract List<Player> GetAllPlayers(int year);
    }
}
