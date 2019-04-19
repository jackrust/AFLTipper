using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;

namespace Universal
{
    public class GeographicallyIndependentTerritory
    {
        public List<Coordinate> TerritoryOutline { get; set; }

        public GeographicallyIndependentTerritory()
        {
            TerritoryOutline = new List<Coordinate>();
        }

        public GeographicallyIndependentTerritory(List<Coordinate> territoryOutline)
        {
            TerritoryOutline = territoryOutline;
        }

        public static string Xmlify(GeographicallyIndependentTerritory geographicallyIndependentTerritory)
        {
            var xml = "";
            xml += "<geographicallyindependentterritory>";
            xml += "<territoryoutline>";
            xml = geographicallyIndependentTerritory.TerritoryOutline.Aggregate(xml, (current, coordinate) => current + Coordinate.Xmlify(coordinate));
            xml += "</territoryoutline>";
            xml += "</geographicallyindependentterritory>";
            return xml;
        }

        public static GeographicallyIndependentTerritory Objectify(string xml)
        {
            var geographicallyIndependentBorderedRegion = Stringy.ExtractSubStringFromBetween(xml, "<geographicallyindependentterritory>", "</geographicallyindependentterritory>");


            var patternCoordinate = @"<territoryoutline>.*?<\/territoryoutline>";
            var rCoordinate = new Regex(patternCoordinate);
            var simpleoutlineXmls = (from object match in rCoordinate.Matches(geographicallyIndependentBorderedRegion) select match.ToString()).ToList();

            var simpleOutline = new List<Coordinate>();

            foreach (var simpleoutlineXml in simpleoutlineXmls)
            {
                var coordinateXml = Stringy.ExtractSubStringFromBetween(simpleoutlineXml, "<territoryoutline>", "</territoryoutline>");
                var coordinate = Coordinate.Objectify(coordinateXml);
                simpleOutline.Add(coordinate);
            }
            return new GeographicallyIndependentTerritory(simpleOutline);
        }
    }
}
