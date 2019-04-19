using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Universal
{
    public class Region
    {
        public string Name { get; set; }
        public State State { get; set; }
        public static Region Melbourne = new Region("Melbourne", State.Victoria);

        public Region(string name, State state)
        {
            Name = name;
            State = state;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && Equals((State)other);
        }

        protected bool Equals(Region other)
        {
            return string.Equals(Name, other.Name) && Equals(State, other.State);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Name != null ? Name.GetHashCode() : 0) * 397) ^ (State != null ? State.GetHashCode() : 0);
            }
        }
    }
}
