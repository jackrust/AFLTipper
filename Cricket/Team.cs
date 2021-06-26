using AustralianRulesFootball;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cricket
{
    public class Team
    {
        public int Id;
        public List<string> Names;
        public Ground Ground;

        #region Static methods
        public static Team FindByName(string name)
        {
            return Teams.First(t => t.Names.Any(n => n.Equals(name.Trim(), StringComparison.InvariantCultureIgnoreCase)));
        }

        public bool Equals(Team other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || this.Id == other.Id;
        }
        #endregion

        #region teams
        public static Team Strikers = new Team {Id = 1, Names = new List<string> {"Adelaide", "Strikers", "Adelaide Strikers"}, Ground = Ground.Ao};
        public static Team Heat = new Team {Id = 2, Names = new List<string> {"Brisbane", "Heat", "Brisbane Heat"}, Ground = Ground.G};
        public static Team Hurricanes = new Team {Id = 3, Names = new List<string> {"Hobart", "Hurricanes", "Hobart Hurricanes"}, Ground = Ground.Ba};
        public static Team Renegades = new Team {Id = 4, Names = new List<string> {"Melbourne", "Renegades", "Melbourne Renegades"}, Ground = Ground.Es};
        public static Team Stars = new Team {Id = 5, Names = new List<string> {"Melbourne", "Stars", "Melbourne Stars"}, Ground = Ground.Mcg};
        public static Team Scorchers = new Team {Id = 6, Names = new List<string> {"Perth", "Scorchers", "Perth Scorchers"}, Ground = Ground.Waca};
        public static Team Sixers = new Team {Id = 7, Names = new List<string> {"Sydney", "Sixers", "Sydney Sixers"}, Ground = Ground.Scg};
        public static Team Thunder = new Team {Id = 8, Names = new List<string> {"Sydney", "Thunder", "Sydney Thunder"}, Ground = Ground.Anz};

        public static List<Team> Teams = new List<Team>{Strikers, Heat, Hurricanes, Renegades, Stars, Scorchers, Sixers, Thunder}; 
        #endregion
    }
}
