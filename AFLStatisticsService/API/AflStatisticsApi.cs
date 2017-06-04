using System.Collections.Generic;
using AustralianRulesFootball;

namespace AFLStatisticsService.API
{
    public abstract class AflStatisticsApi
    {
        protected static readonly Dictionary<int, int> numHomeandAwayRounds = new Dictionary<int, int>
        {   
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

        public abstract int GetNumRounds(int year);

        public abstract Round GetRoundResults(int year, int roundNo);

        public abstract List<Player> GetAllPlayers(int year);
    }
}
