namespace AustralianRulesFootball
{
    public class RoundShell : Entity
    {
        public int Year;
        public int Number;
        public bool IsFinal;

        public RoundShell()
        {
            Number = 0;
            Year = 0;
            IsFinal = false;
    }

        public RoundShell(int year, int number, bool isFinal)
        {
            Number = number;
            Year = year;
            IsFinal = isFinal;
        }

        public int EffectiveId()
        {
            return (Year * 1000) + (IsFinal ? 100 : 0) + (Number);
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
            return new RoundShell(r.Year, r.Number + i, r.IsFinal);
        }

        public static RoundShell operator -(RoundShell r, int i)
        {
            return new RoundShell(r.Year, r.Number - i, r.IsFinal);
        }
    }
}
