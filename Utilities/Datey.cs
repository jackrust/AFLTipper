using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class Datey
    {
        public enum DateAccuracy
        {
            Millisecond,
            Second,
            Minute,
            Hour,
            Day,
            Month,
            Year
        }

        public static bool Approximates(DateTime one, DateTime two, DateAccuracy accuracy = DateAccuracy.Day)
        {
            var approximate = (one.Year == two.Year);
            if (accuracy == DateAccuracy.Year) return approximate;
            approximate = approximate && (one.Month == two.Month);
            if (accuracy == DateAccuracy.Month) return approximate;
            approximate = approximate && (one.Day == two.Day);
            if (accuracy == DateAccuracy.Day) return approximate;
            approximate = approximate && (one.Hour == two.Hour);
            if (accuracy == DateAccuracy.Hour) return approximate;
            approximate = approximate && (one.Minute == two.Minute);
            if (accuracy == DateAccuracy.Minute) return approximate;
            approximate = approximate && (one.Second == two.Second);
            if (accuracy == DateAccuracy.Second) return approximate;
            approximate = approximate && (one.Millisecond == two.Millisecond);

            return approximate;
        }
    }
}
