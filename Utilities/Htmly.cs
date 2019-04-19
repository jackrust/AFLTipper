using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Utilities.Html;

namespace Utilities
{
    public class Htmly
    {
        public static Element ExtractElements(string html)
        {
            var element = new Element();

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);
            return element;
        }

        public static string FindCostIn(string html, string option)
        {
            //TODO: convert option to options
            var result = "";
            var regex = new Regex(@"[$][0-9]{1,3}(?:,?[0-9]{3})*(\.[0-9]{2})*");
            var index = html.IndexOf(option, StringComparison.Ordinal);
            var matchone = regex.Match(html);

            foreach (Match match in regex.Matches(html))
            {
                if (result == "" && match.Index > index)
                {
                    result = match.Value;
                }
            }

            return result;
        }
    }

    public class HtmlySearchInformation
    {
        public string keywords;
    }

    public class HtmlyResultDetails
    {
        public string Cost;
        public string Weight;
    }

    public class HtmlyResultDetail
    {
        public List<string> Names;
        public Regex Format;
        public string Value;
    }
}
