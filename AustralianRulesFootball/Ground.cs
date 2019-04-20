using System;
using System.Collections.Generic;
using System.Linq;
using Universal;
using Utilities;

namespace AustralianRulesFootball
{
    public class Ground : Entity
    {
        public string Abbreviation;
        public State State;
        public GeographicCoordinate Coordinate;
        public List<string> Names;
        public List<string> Abbreviations;

        public Ground(string abbreviation, List<string> names, List<string> abbreviations, State state, GeographicCoordinate coordinate)
        {
            if (abbreviation == "")
                Abbreviation = "WTF";
            Abbreviation = abbreviation;
            State = state;
            Coordinate = coordinate;
            Names = names;
            Abbreviations = abbreviations;
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.GetType() == GetType() && Equals((Ground) other);
        }

        public bool Equals(Ground other)
        {
            if (ReferenceEquals(null, other)) return false;
            return ReferenceEquals(this, other) || String.Equals(Abbreviation, other.Abbreviation);
        }
        #region IO
        public string Stringify()
        {
            var s = "";
            s += "<abbreviation>" + Abbreviation + "</abbreviation>";
            return s;
        }

        public static Ground Objectify(string str)
        {

            var a = Stringy.SplitOn(str, "abbreviation")[0];
            var ground = Util.GetGrounds().FirstOrDefault(g => g.Abbreviation == a);
            return ground;
        }

        public static Ground LoadByAbbreviation(string abbreviation)
        {
            var ground = Util.GetGrounds().FirstOrDefault(g => g.Abbreviation == abbreviation || g.Abbreviations.Contains(abbreviation));
            if (ground == null)
                Console.WriteLine("Error! Ground not found: " + abbreviation);
            return ground;
        }

        public override int GetHashCode()
        {
            return (Abbreviation != null ? Abbreviation.GetHashCode() : 0);
        }
        #endregion

        //AustralianCapitalTerritory
        public static Ground Bs = new Ground("BS", new List<string> {"Bruce Stadium"}, new List<string> {}, State.AustralianCapitalTerritory, new GeographicCoordinate(-35.2500, 149.1028));
        public static Ground Sto = new Ground("STO", new List<string> { "Manuka Oval", "Manuka", "MO", "StarTrack Oval", "StarTrack Ovak", "StarTrack Oval Canberra" }, new List<string> { }, State.AustralianCapitalTerritory, new GeographicCoordinate(-35.318056, 149.134722));
        
        //NewSouthWales
        public static Ground Anz = new Ground("ANZ", new List<string> { "ANZ Stadium", "Stadium Australia", "Telstra Stadium" }, new List<string> { }, State.NewSouthWales, new GeographicCoordinate(-33.8472, 151.0633));
        public static Ground Bisp = new Ground("BISP", new List<string> { "Blacktown ISP", "Blacktown Park" }, new List<string> { }, State.NewSouthWales, new GeographicCoordinate(-33.76944444, 150.85916667));
        public static Ground Scg = new Ground("SCG", new List<string> { "Sydney Cricket Ground" }, new List<string> { }, State.NewSouthWales, new GeographicCoordinate(-33.8917, 151.2247));
        public static Ground Sss = new Ground("SSS", new List<string> { "Sydney Showground Stadium" }, new List<string> { }, State.NewSouthWales, new GeographicCoordinate(-33.8433183, 151.0667875));
        
        //NorthernTerritory
        public static Ground Tio = new Ground("TIO", new List<string> { "TIO Stadium", "Marrara" }, new List<string> { }, State.NorthernTerritory, new GeographicCoordinate(-12.3992, 130.8872));
        public static Ground Ttp = new Ground("TTP", new List<string> { "TIO Trager Park", "TIO Traeger Park", "Traeger Park", "TP" }, new List<string> { }, State.NorthernTerritory, new GeographicCoordinate(-23.7090, 133.8751));
        
        //Queensland
        public static Ground Cs = new Ground("CS", new List<string> { "Cazaly's Stadium", "Cazaly�s Stadium" }, new List<string> { }, State.Queensland, new GeographicCoordinate(-16.9358, 145.7492));
        public static Ground G = new Ground("G", new List<string> { "Gabba" }, new List<string> { }, State.Queensland, new GeographicCoordinate(-27.4858, 153.0381));
        public static Ground Ms = new Ground("MS", new List<string> { "Metricon Stadium", "Carrara", "Laver Oval", "Gold Coast Stadium" }, new List<string> { }, State.Queensland, new GeographicCoordinate(28.0064, 153.3672));
        public static Ground Sps = new Ground("SPS", new List<string> { "Spotless Stadium", "Skoda Stadium", "SPO" }, new List<string> { }, State.Queensland, new GeographicCoordinate(-33.8431, 151.0678));
        public static Ground Rs = new Ground("RS", new List<string> { "Riverway Stadium" }, new List<string> { }, State.Queensland, new GeographicCoordinate(-19.3178115, 146.7317297));
        
        //SouthAustralia
        public static Ground A = new Ground("A", new List<string> { "AAMI Stadium", "Football Park" }, new List<string> { }, State.SouthAustralia, new GeographicCoordinate(-34.8800, 138.4956));
        public static Ground Ao = new Ground("AO", new List<string> {"Adelaide Oval"}, new List<string> {}, State.SouthAustralia, new GeographicCoordinate(-34.9156, 138.5961));
        
        //Tasmania
        public static Ground Au = new Ground("AU", new List<string> { "Aurora Stadium", "York Park", "University of Tasmania", "University of Tasmania Stadium" }, new List<string> { "US" }, State.Tasmania, new GeographicCoordinate(-41.4258, 147.1389));
        public static Ground Ba = new Ground("BA", new List<string> {"Bludstone Arena", "Blundstone Arena"}, new List<string> {}, State.Tasmania, new GeographicCoordinate(-42.877699, 147.373535));
        public static Ground Nh = new Ground("NH", new List<string> {"North Hobart"}, new List<string> {}, State.Tasmania, new GeographicCoordinate(-42.867778, 147.315833));
        
        //Victoria
        public static Ground P = new Ground("P", new List<string> {"AFL Park", "Waverly Park"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.925556, 145.188611));
        public static Ground As = new Ground("AS", new List<string> {"Arden Street"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.7992, 144.9411));
        public static Ground Es = new Ground("ES", new List<string> { "Etihad Stadium", "Colonial Stadium", "Telstra Dome", "Marvel Stadium" }, new List<string> { }, State.Victoria, new GeographicCoordinate(-37.8164, 144.9475));
        public static Ground Eu = new Ground("EU", new List<string> { "Eureka Stadium", "Northern Oval #1", "AUSTAR" }, new List<string> { }, State.Victoria, new GeographicCoordinate(-37.539322, 143.848102));
        public static Ground Jo = new Ground("JO", new List<string> {"Junction Oval"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.7803, 145.6127));
        public static Ground Lo = new Ground("LO", new List<string> {"Lake Oval", "Lakeside Stadium"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.840278, 144.965));
        public static Ground Lho = new Ground("LHO", new List<string> {"Linen House Oval", "Moorabbin Oval"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.9375, 145.0439));
        public static Ground Mclp = new Ground("MSLP", new List<string> {"MC Labour Park", "Optus Oval", "Princes Park", "Visy Park", "Ikon Park"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.783889, 144.961667));
        public static Ground Mcg = new Ground("MCG", new List<string> {"Melbourne Cricket Ground"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.8200, 144.9833));
        public static Ground Ss = new Ground("SS", new List<string> {"Simonds Stadium", "Skilled Stadium", "Shell Stadium", "Kardinia Park"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-38.1581, 144.3547));
        public static Ground V = new Ground("V", new List<string> {"Victoria Park"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.798333, 144.996389));
        public static Ground Wo = new Ground("WO", new List<string> {"Whitten Oval"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.7992, 144.8867));
        public static Ground Wh = new Ground("WH", new List<string> {"Windy Hill"}, new List<string> {}, State.Victoria, new GeographicCoordinate(-37.751944, 144.919722));
        
        //WesternAustralia
        public static Ground Ps = new Ground("PS", new List<string> { "Patersons Stadium", "Subiaco Oval", "Domain Stadium", "DS" }, new List<string> { }, State.WesternAustralia, new GeographicCoordinate(-31.9444, 115.8300));
        public static Ground Waca = new Ground("WACA", new List<string> {"W.A.C.A", "W.A.C.A."}, new List<string> {}, State.WesternAustralia, new GeographicCoordinate(-31.9600, 115.8797));
        public static Ground Os = new Ground("OS", new List<string> { "Optus Stadium", "Perth Stadium" }, new List<string> { }, State.WesternAustralia, new GeographicCoordinate(-31.9513567, 115.888865));

        //North Island
        public static Ground Ws = new Ground("WC", new List<string> { "Wespac Stadium", "Westpac Stadium"}, new List<string> { }, State.NorthIsland, new GeographicCoordinate(-41.2731, 174.7858));

        //China
        public static Ground Jss = new Ground("JSS", new List<string> { "Jiangwan Sports Stadium", "Shanghai Stadium" }, new List<string> { "JS" }, State.Shanghai, new GeographicCoordinate(31.307546, 121.518601));
    }
}
