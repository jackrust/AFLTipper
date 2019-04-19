using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Utilities
{
    public class Filey
    {
        public static void Save(List<List<double>> values, string fileName)
        {
            var lines = values.Select(v => v.Aggregate("", (current, d) => current + (d + ","))).Select(line => line.TrimEnd(',')).ToList();
            Save(lines, fileName);
        }

        public static void Save(Dictionary<string, double> values, string fileName)
        {
            var lines = values.Select(v => v.Key + "," + v.Value).ToList();
            Save(lines, fileName);
        }

        public static void Save(List<string> lines, string fileName)
        {
            var file = new StreamWriter(fileName);
            foreach (var l in lines)
            {
                file.WriteLine(l);
            }
            file.Close();
        }

        public static void Save(string line, string fileName)
        {
            var file = new StreamWriter(fileName);
            file.WriteLine(line);
            file.Close();
        }

        public static void Append(string line, string fileName)
        {
            var file = File.AppendText(fileName);
            file.WriteLine(line);
            file.Close();
        }

        public static string Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                var lines = File.ReadAllText(fileName);
                return lines;
            }
            return "";
        }

        public static List<string> LoadLines(string filePath)
        {
            var lines = new List<string>();
            if (!File.Exists(filePath)) return lines;

            string line;
            var file = new StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                lines.Add(line);
            }

            return lines;
        }

        public static string LoadHexData(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open);
            int hexIn;
            string hex = "";

            for (int i = 0; (hexIn = fs.ReadByte()) != -1; i++)
            {
                hex += string.Format("{0:X2}", hexIn);
            }
            return hex;
        }

        public static string FindFile(string path, string fileName)
        {
            string file;
            try
            {
                file = Directory.GetFiles(path, fileName, SearchOption.AllDirectories)
                    .FirstOrDefault();
            }
            catch (Exception)
            {
                file = null;
            }

            return file;
        }
    }
}
