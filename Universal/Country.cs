using System;

namespace Universal
{
    public class Country
    {
        

        public string Name { get; set; }
        public static Country Australia = new Country("Australia");
        public static Country NewZealand = new Country("New Zealand");

        public Country(string name)
        {
            Name = name;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && Equals((Country)other);
        }

        public bool Equals(Country other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return String.Equals(Name, other.Name);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}
