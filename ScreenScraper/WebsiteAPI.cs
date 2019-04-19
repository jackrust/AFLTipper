using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;

namespace ScreenScraper
{
    public abstract class WebsiteAPI
    {
        public static string GetPage(string page, Dictionary<string, string> parameters)
        {
            var url = page + "?";
            url = parameters.Aggregate(url, (current, p) => current + (p.Key + "=" + p.Value + "&"));
            url = url.TrimEnd('&');

            return GetPage(url);
        }

        public static string GetPage(string url)
        {
            var client = new WebClient();
            Stream data = null;
            try
            {
                data = client.OpenRead(url);
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            if (data == null) return "";
            var reader = new StreamReader(data);
            var html = reader.ReadToEnd();
            data.Close();
            reader.Close();

            return html;
        }

        public static List<string> SplitOn(string main, string start, string end)
        {
            return SplitOn(main, start, end, "", 0);
        }

        public static List<string> SplitOn(string main, string start, string end, int frontJunk)
        {
            return SplitOn(main, start, end, "", frontJunk);
        }

        public static List<string> SplitOn(string main, string start, string end, string identifier)
        {
            return SplitOn(main, start, end, identifier, 0);
        }

        public static List<string> SplitOn(string main, string start, string end, string identifier, int frontJunk)
        {
            var rows = new List<string>();
            var sflag = main.IndexOf(start, StringComparison.Ordinal);
            if (sflag < 0) return rows;
            var iflag = main.IndexOf(identifier, sflag, StringComparison.Ordinal);
            if (iflag < 0) return rows;
            var eflag = main.IndexOf(end, iflag, StringComparison.Ordinal);
            if (eflag < 0) return rows;

            while (sflag > -1 && iflag > -1 && eflag > -1)
            {
                rows.Add(main.Substring(sflag + frontJunk, eflag - (sflag + frontJunk)));
                sflag = main.IndexOf(start, eflag + 1, StringComparison.Ordinal);
                iflag = main.IndexOf(identifier, sflag + 1, StringComparison.Ordinal);
                eflag = main.IndexOf(end, iflag + 1, StringComparison.Ordinal);
            }
            return rows;
        }
    }
}
