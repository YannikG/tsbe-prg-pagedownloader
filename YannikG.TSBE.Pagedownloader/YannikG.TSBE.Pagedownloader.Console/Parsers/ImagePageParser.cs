using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace YannikG.TSBE.Pagedownloader.Console.Parsers
{
    public class ImagePageParser
    {
        private const string HTML_ELEMENT_IMG = "img";
        private const string HTML_IMG_SRC = "src";

        public static List<string> Parse(string html, string baseUrl)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            List<string> result = new List<string>();

            var nextPages = htmlDoc.DocumentNode.Descendants(HTML_ELEMENT_IMG)
                .ToList()
                    .Select(node => node.GetAttributeValue(HTML_IMG_SRC, string.Empty))
                    .Where(url => !string.IsNullOrEmpty(url))
                .ToList();

            result.AddRange(nextPages.Where(url => url.Contains(baseUrl)));
            result.AddRange(
                nextPages
                    .Where(url => url.StartsWith("/") || !url.StartsWith("http"))
                    .Select(url =>
                        baseUrl + (baseUrl.Last() == '/' && url.StartsWith('/') ? url.TrimStart('/') : $"/{url}")
                           ));

            result = result.Distinct().ToList();

            return result;
        }
    }
}