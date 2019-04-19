using System;
using Utilities;

namespace Universal
{
    public class Coordinate
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Coordinate()
        {
            X = 0;
            Y = 0;
        }

        public Coordinate(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Coordinate operator +(Coordinate c1, Coordinate c2)
        {
            return new Coordinate(c1.X + c2.X, c1.Y + c2.Y);
        }

        public static string Xmlify(Coordinate coordinate)
        {
            var xml = "";
            xml += "<coordinate>";
            xml += "<x>" + coordinate.X + "</x>";
            xml += "<y>" + coordinate.Y + "</y>";
            xml += "</coordinate>";
            return xml;
        }

        public static Coordinate Objectify(string xml)
        {
            var coordinate = Stringy.ExtractSubStringFromBetween(xml, "<coordinate>", "</coordinate>");
            var x = Stringy.ExtractSubStringFromBetween(coordinate, "<x>", "</x>");
            var y = Stringy.ExtractSubStringFromBetween(coordinate, "<y>", "</y>");
            return new Coordinate(Double.Parse(x), Double.Parse(y));
        }
    }
}
