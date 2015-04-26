using System;

namespace Universal
{
    public class State
    {
        public string Name { get; set; }
        public Country Country { get; set; }
        public static State AustralianCapitalTerritory = new State("Australian Capital Territory", Country.Australia);
        public static State NewSouthWales = new State("New South Wales", Country.Australia);
        public static State NorthernTerritory = new State("Northern Territory", Country.Australia);
        public static State Queensland = new State("Queensland", Country.Australia);
        public static State SouthAustralia = new State("South Australia", Country.Australia);
        public static State Tasmania = new State("Tasmania", Country.Australia);
        public static State Victoria = new State("Victoria", Country.Australia);
        public static State WesternAustralia = new State("WesternAustralia", Country.Australia);

        public static State NorthIsland = new State("North Island", Country.NewZealand);
        public static State SouthIsland = new State("South Island", Country.NewZealand);

        public State(string name, Country country)
        {
            Name = name;
            Country = country;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && Equals((State)other);
        }

        public bool Equals(State other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (Country != null ? Country.GetHashCode() : 0);
            }
        }
    }
}
