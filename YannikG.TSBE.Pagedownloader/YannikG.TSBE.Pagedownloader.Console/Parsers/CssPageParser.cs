using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using HtmlAgilityPack;

namespace YannikG.TSBE.Pagedownloader.Console.Parsers
{
    public static class CssPageParser
    {
        private const string HTML_ELEMENT_LINK = "link";
        private const string HTML_LINK_HREF = "href";
        private const string HTML_LINK_REL = "rel";
        private const string HTML_LINK_REL_TYPE_STYLESHEET = "stylesheet";

        public static List<string> Parse(string html, string baseUrl)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            List<string> result = new List<string>();

            var nexCSS = htmlDoc.DocumentNode.Descendants(HTML_ELEMENT_LINK)
                .ToList()
                    .Where(node => node.GetAttributeValue(HTML_LINK_REL, string.Empty) == HTML_LINK_REL_TYPE_STYLESHEET)
                    .Select(node => node.GetAttributeValue(HTML_LINK_HREF, string.Empty))
                    .Where(url => !string.IsNullOrEmpty(url))
                .ToList();

            result.AddRange(nexCSS.Where(url => url.Contains(baseUrl)));
            result.AddRange(
                nexCSS
                    .Where(url => url.StartsWith("/") || !url.StartsWith("http"))
                    .Select(url =>
                        baseUrl + (baseUrl.Last() == '/' && url.StartsWith('/') ? url.TrimStart('/') : $"/{url}")
                           ));

            result = result.Distinct().ToList();

            return result;
        }
    }
}