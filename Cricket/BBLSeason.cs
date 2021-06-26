using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;

namespace Cricket
{
    public class BBLSeason : Entity
    {
        //https://www.mykhel.com/cricket/big-bash-league-2011-12-schedule-results-s9954/

        public int Year; //Start year
        public List<Match> Matches;

        public BBLSeason():base()
        {
            Year = 0;
            Matches = new List<Match>();
        }

        public BBLSeason(int year, List<Match> matches)
        {
            Year = year;
            Matches = matches;
        }
    }
}
