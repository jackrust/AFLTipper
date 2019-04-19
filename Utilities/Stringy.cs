using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Utilities
{
    public class Stringy
    {
        public static string ExtractSubStringFromBetween(string str, string before, string after)
        {
            var start = str.IndexOf(before, StringComparison.Ordinal) + before.Length;
            var end = str.IndexOf(after, start, StringComparison.Ordinal);
            var sub = str.Substring(start, end - start);
            return sub;
        }

        public static List<string> SplitOn(string str, string tag)
        {
            string start = "<" + tag.ToUpper() + ">";
            string end = "</" + tag.ToUpper() + ">";
            List<string> strs = new List<string>();
            int sflag = str.ToUpper().IndexOf(start);
            if (sflag == -1)
                return new List<string>() { "0" };
            int eflag = str.ToUpper().IndexOf(end, sflag);
            if (eflag == -1)
                return new List<string>() { "0" };

            while (sflag > -1 && eflag > -1)
            {
                strs.Add(str.Substring(sflag + start.Length, eflag - (sflag + start.Length)));
                sflag = str.ToUpper().IndexOf(start, eflag + 1);
                eflag = str.ToUpper().IndexOf(end, sflag + 1);
            }
            if (strs.Count == 0)
                strs.Add("0");
            return strs;
        }

        public static List<string> Lengthen(List<string> strs, int length)
        {
            return Lengthen(strs, length, ' ');
        }

        public static List<string> Lengthen(List<string> strs, int length, char paddingChar)
        {
            List<string> outStrings = new List<string>();
            foreach(var s in strs)
                outStrings.Add(Lengthen(s, length, paddingChar));
            return outStrings;
        }

        public static string Lengthen(string str, int length)
        {
            return Lengthen(str, length, ' ');
        }

        public static string Lengthen(string str, int length, char paddingChar)
        {
            var outString = str;
            var currentLength = str.Length;
            for (var i = 0; i < length - currentLength; i++)
                outString += paddingChar;
            return outString;
        }

        /*untested*/
        public static string XmlSerializeToString(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }

        public static T XmlDeserializeFromString<T>(string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }

        public static double Compare(string one, string two)
        {
            if (one == null || two == null)
                return -1;
            //same
            if (one.Equals(two))
                return 1;
            //one inside the other
            if (one.Contains(two))
            {
                //% match
                return (double)two.Length / (double)one.Length;
            }
            //one inside the other
            if (two.Contains(one))
            {
                //% match
                return (double)one.Length / (double)two.Length;
            }
            //partial match
            if (one.ToCharArray().Intersect(two.ToCharArray()).Any())
            {
                //% match
                return (double)one.ToCharArray().Intersect(two.ToCharArray()).Count() * 2 /
                       ((double)one.Length + (double)two.Length);
            }
            //no match
            return 0;
        }

        public static double Equalish(string one, string two)
        {
            return (double)one.ToCharArray().Intersect(two.ToCharArray()).Count() * 2 /
                       ((double)one.Length + (double)two.Length);
        }
    }
}
