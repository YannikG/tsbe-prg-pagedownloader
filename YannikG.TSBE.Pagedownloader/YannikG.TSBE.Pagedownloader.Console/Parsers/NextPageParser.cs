using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using HtmlAgilityPack;

namespace YannikG.TSBE.Pagedownloader.Console.Parsers
{
    public static class NextPageParser
    {
        private const string HTML_ELEMENT_NEXT_PAGE = "a";
        private const string HTML_A_HREF = "href";

        public static List<string> Parse(string html, string baseUrl)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            List<string> result = new List<string>();

            var rawNextPageUrls = htmlDoc.DocumentNode.Descendants(HTML_ELEMENT_NEXT_PAGE)
                .ToList()
                    .Select(node => node.GetAttributeValue(HTML_A_HREF, string.Empty))
                    .Where(url => !string.IsNullOrEmpty(url))
                .ToList();

            result.AddRange(rawNextPageUrls.Where(url => url.Contains(baseUrl)));
            result.AddRange(
                rawNextPageUrls
                    .Where(url => url.StartsWith("/"))
                    .Select(url =>
                        baseUrl + (baseUrl.Last() == '/' ? url.TrimStart('/') : url)
                           ));

            result = result.Distinct().ToList();

            return result;
        }
    }
}