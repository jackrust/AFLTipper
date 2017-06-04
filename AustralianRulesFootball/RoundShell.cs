namespace AustralianRulesFootball
{
    public class RoundShell
    {
        public int Year;
        public int Number;

        public RoundShell()
        {
            Number = 0;
            Year = 0;
        }

        public RoundShell(int year, int number)
        {
            Number = number;
            Year = year;
        }

        public bool Equals(RoundShell other)
        {
            if (other == null)
                return false;
            return Year.Equals(other.Year)
                && Number.Equals(other.Number);
        }

        public static RoundShell operator +(RoundShell r, int i)
        {
            return new RoundShell(r.Year, r.Number + i);
        }

        public static RoundShell operator -(RoundShell r, int i)
        {
            return new RoundShell(r.Year, r.Number - i);
        }
    }
}
