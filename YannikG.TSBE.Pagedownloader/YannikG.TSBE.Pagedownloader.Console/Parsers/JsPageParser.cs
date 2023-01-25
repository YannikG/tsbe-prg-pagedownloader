using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace YannikG.TSBE.Pagedownloader.Console.Parsers
{
    public class JsPageParser
    {
        private const string HTML_ELEMENT_SCRIPT = "script";
        private const string HTML_SCRIPT_SRC = "src";

        public static List<string> Parse(string html, string baseUrl)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            List<string> result = new List<string>();

            var nextJs = htmlDoc.DocumentNode.Descendants(HTML_ELEMENT_SCRIPT)
                .ToList()
                    .Select(node => node.GetAttributeValue(HTML_SCRIPT_SRC, string.Empty))
                    .Where(url => !string.IsNullOrEmpty(url))
                .ToList();

            result.AddRange(nextJs.Where(url => url.Contains(baseUrl)));
            result.AddRange(
                nextJs
                    .Where(url => url.StartsWith("/") || !url.StartsWith("http"))
                    .Select(url =>
                        baseUrl + (baseUrl.Last() == '/' && url.StartsWith('/') ? url.TrimStart('/') : $"/{url}")
                           ));

            result = result.Distinct().ToList();

            return result;
        }
    }
}