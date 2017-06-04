using System;
using System.Collections.Generic;
using Utilities;

namespace AustralianRulesFootball
{
    public class Team
    {
        public readonly string Region;
        public readonly string Mascot;
        public readonly string ApiName;
        public readonly List<String> Names;
        public readonly List<Ground> Homes;
        public readonly List<Colour> Colours;

        public Team(string region, string mascot, string apiName, Ground home, List<Colour> colours)
            : this(region, mascot, apiName, new List<string>(), new List<Ground>() {home}, colours)
        { }

        public Team(string region, string mascot, string apiName, List<string> otherNames, List<Ground> homes, List<Colour> colours)
        {
            Region = region;
            Mascot = mascot;
            ApiName = apiName;
            Names = new List<string>{Region, Mascot, ApiName};
            Names.AddRange(otherNames);
            Homes = homes;
            Colours = colours;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && Equals((Team) other);
        }

        public bool Equals(Team other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || String.Equals(ApiName, other.ApiName);
        }

        #region IO
        public String Stringify()
        {
            var s = "";
            s += "<APIName>" + ApiName + "</APIName>";
            return s;
        }

        public static Team Objectify(string str)
        {
            var apiName = Stringy.SplitOn(str, "APIName")[0];
            return Util.GetTeamByName(apiName);
        }

        public static Team LoadByName(string name)
        {
            return Util.GetTeamByName(name);
        }

        public override int GetHashCode()
        {
            return (ApiName != null ? ApiName.GetHashCode() : 0);
        }
        #endregion

        #region Data
        public static Team Adelaide = new Team("Adelaide", "Crows", "Adelaide", new List<string>{"Adelaide Crows"}, new List<Ground>() { Ground.Ao},
            new List<Colour>() { Colour.NavyBlue, Colour.Red, Colour.Gold });

        public static Team Brisbane = new Team("Brisbane", "Lions", "Brisbane Lions", Ground.G,
            new List<Colour>() { Colour.Maroon, Colour.Blue, Colour.Gold });

        public static Team Carlton = new Team("Carlton", "Blues", "Carlton", new List<string>(), new List<Ground>() { Ground.Es, Ground.Mcg },
            new List<Colour>() { Colour.NavyBlue, Colour.White });

        public static Team Collingwood = new Team("Collingwood", "Magpies", "Collingwood", Ground.Mcg,
            new List<Colour>() { Colour.Black, Colour.White });

        public static Team Essendon = new Team("Essendon", "Bombers", "Essendon", new List<string>(), new List<Ground>() { Ground.Es, Ground.Mcg },
            new List<Colour>() { Colour.Red, Colour.Black });

        public static Team Fremantle = new Team("Fremantle", "Dockers", "Fremantle", new List<string>(), new List<Ground>() { Ground.Ps, Ground.Waca },
            new List<Colour>() { Colour.Purple, Colour.White });

        public static Team Geelong = new Team("Geelong", "Cats", "Geelong", new List<string> { "Geelong Cats" }, new List<Ground>() { Ground.Ss }, new List<Colour>() { Colour.NavyBlue, Colour.White });

        public static Team GoldCoast = new Team("Gold Coast", "Suns", "Gold Coast", new List<string>{"Gold Coast Suns"},new List<Ground>() {  Ground.Ms},
            new List<Colour>() { Colour.Red, Colour.Gold, Colour.Blue });

        public static Team GreaterWesternSydney = new Team("Greater Western Sydney", "Giants", "GWS Giants", new List<string>(),
            new List<Ground>() { Ground.Sps, Ground.Anz, Ground.Sto }, new List<Colour>() { Colour.Orange, Colour.Charcoal, Colour.White });

        public static Team Hawthorn = new Team("Hawthorn", "Hawks", "Hawthorn", new List<string>(), new List<Ground>() { Ground.Mcg, Ground.Au },
            new List<Colour>() { Colour.Brown, Colour.Gold });

        public static Team Melbourne = new Team("Melbourne", "Demons", "Melbourne", Ground.Mcg,
            new List<Colour>() { Colour.NavyBlue, Colour.Red });

        public static Team NorthMelbourne = new Team("North Melbourne", "Kangaroos", "North Melbourne", Ground.Es,
            new List<Colour>() { Colour.RoyalBlue, Colour.White });

        public static Team PortAdelaide = new Team("Port Adelaide", "Power", "Port Adelaide", Ground.Ao,
            new List<Colour>() { Colour.Black, Colour.White, Colour.Teal, Colour.Silver });

        public static Team Richmond = new Team("Richmond", "Tigers", "Richmond", Ground.Mcg, new List<Colour>() { Colour.Yellow, Colour.Black });

        public static Team StKilda = new Team("St. Kilda", "Saints", "St Kilda", new List<string>(), new List<Ground>() { Ground.Es, Ground.Mcg },
            new List<Colour>() { Colour.Red, Colour.White, Colour.Black });

        public static Team Sydney = new Team("Sydney", "Swans", "Sydney", new List<string>() { "Sydney Swans" }, new List<Ground>() { Ground.Scg, Ground.Anz },
            new List<Colour>() { Colour.Red, Colour.White });

        public static Team WestCoast = new Team("West Coast", "Eagles", "West Coast", new List<string> { "West Coast Eagles" }, new List<Ground>() { Ground.Waca},
            new List<Colour>() { Colour.NavyBlue, Colour.White, Colour.Gold });

        public static Team Western = new Team("Western", "BullDogs", "Western Bulldogs", Ground.Es,
            new List<Colour>() { Colour.Red, Colour.White, Colour.Blue });
        #endregion
    }
}
