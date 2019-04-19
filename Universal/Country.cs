using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Utilities;

namespace Universal
{
    //TODO: this classification is stupid
    public enum ColorDifferentiator
    {
        Unclassified = 0,
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
    }
    public class Country
    {
        public string Name { get; set; }
        public string Alpha2Code { get; set; }
        //public string Alpha3Code { get; set; }
        public ColorDifferentiator Differentiator { get; set; }
        public List<GeographicallyIndependentTerritory> CountryOutline { get; set; }

        

        public Country(string name)
        {
            Name = name;
            CountryOutline = new List<GeographicallyIndependentTerritory>();
        }

        public Country(string name, string alpha2Code, ColorDifferentiator differentiator, List<GeographicallyIndependentTerritory> countryOutline)
        {
            Name = name;
            Alpha2Code = alpha2Code;
            Differentiator = differentiator;
            CountryOutline = countryOutline;
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

        #region Countries
        public static Country Australia = new Country("Australia");
        public static Country NewZealand = new Country("New Zealand");
        public static Country China = new Country("China");
        #endregion

        public static string Xmlify(Country country)
        {
            var xml = "";
            xml += "<country>";
            xml += "<name>" + country.Name + "</name>";
            xml += "<alpha2code>" + country.Alpha2Code + "</alpha2code>";
            xml += "<differentiator>" + country.Differentiator + "</differentiator>";
            xml += "<countryoutline>";
            xml = country.CountryOutline.Aggregate(xml, (current, gibr) => current + GeographicallyIndependentTerritory.Xmlify(gibr));
            xml += "</countryoutline>";
            xml += "</country>";
            return xml;
        }

        public static Country Objectify(string xml)
        {
            var country = Stringy.ExtractSubStringFromBetween(xml, "<country>", "</country>");

            var name = Stringy.ExtractSubStringFromBetween(country, "<name>", "</name>");
            var alpha2code = Stringy.ExtractSubStringFromBetween(country, "<alpha2code>", "</alpha2code>");
            ColorDifferentiator differentiator = (ColorDifferentiator)Int32.Parse(Stringy.ExtractSubStringFromBetween(country, "<differentiator>", "</differentiator>"));

            
            var patternCountryoutline = @"<countryoutline>.*?<\/countryoutline>";
            var rCountryoutline = new Regex(patternCountryoutline);
            var countryoutlineXmls = (from object match in rCountryoutline.Matches(country) select match.ToString()).ToList();
            var countryOutline = new List<GeographicallyIndependentTerritory>();
            foreach (var countryOutlineXml in countryoutlineXmls)
            {
                var geographicallyIndependentTerritoryXml = Stringy.ExtractSubStringFromBetween(countryOutlineXml, "<countryoutline>", "</countryoutline>");
                var geographicallyIndependentTerritory = GeographicallyIndependentTerritory.Objectify(geographicallyIndependentTerritoryXml);
                countryOutline.Add(geographicallyIndependentTerritory);
            }
            return new Country(name, alpha2code, differentiator, countryOutline);
        }
    }
}
