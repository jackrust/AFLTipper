using System;

namespace AustralianRulesFootball
{
    public class Colour
    {
        public String Name = "";
        public String Hex = "";

        public Colour() { }

        public Colour(String name) :this(name, ""){ }

        public Colour(String name, string hex)
        {
            Name = name;
            Hex = hex;
        }

        public static Colour Black = new Colour("Black", "");
        public static Colour Blue = new Colour("Blue", "");
        public static Colour Brown = new Colour("Brown", "");
        public static Colour Charcoal = new Colour("Charcoal", "");
        public static Colour Gold = new Colour("Gold", "");
        public static Colour Maroon = new Colour("Maroon", "");
        public static Colour NavyBlue = new Colour("Navy Blue", "");
        public static Colour Orange = new Colour("Orange", "");
        public static Colour Purple = new Colour("Purple", "");
        public static Colour Red = new Colour("Red", "");
        public static Colour RoyalBlue = new Colour("Royal Blue", "");
        public static Colour Silver = new Colour("Silver", "");
        public static Colour Teal = new Colour("Teal", "");
        public static Colour White = new Colour("White", "");
        public static Colour Yellow = new Colour("Yellow", "");
    }
}
