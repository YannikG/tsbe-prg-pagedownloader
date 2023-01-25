using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Spectre.Console;
using YannikG.TSBE.Pagedownloader.Console.Access;
using YannikG.TSBE.Pagedownloader.Console.Parsers;

namespace YannikG.TSBE.Pagedownloader.Console
{
    public class DownloaderService
    {
        private readonly HttpAccessHandler httpAccessHandler;
        private readonly FileAccessHandler fileAccessHandler;

        private readonly string rootUrl;

        private List<string> pagesToDo = new List<string>();
        private List<string> pagesDone = new List<string>();

        private List<string> cssToDo = new List<string>();
        private List<string> jsToDo = new List<string>();
        private List<string> imagesToDo = new List<string>();

        private readonly string[] commonImageTypes = { ".png", ".jpg", ".jpeg", ".svg" };

        public DownloaderService(string startUrl, string baseDownloadFolder)
        {
            this.pagesToDo.Add(startUrl);
            this.rootUrl = new Url(startUrl).Root;

            this.httpAccessHandler = new HttpAccessHandler(rootUrl);
            this.fileAccessHandler = new FileAccessHandler(baseDownloadFolder);
        }

        public async Task StartAsync()
        {
            await AnsiConsole.Status()
            .Spinner(Spinner.Known.Earth)
            .StartAsync($"Downloading from '{this.rootUrl}'", async ctx =>
            {
                AnsiConsole.MarkupLine("Downloading Pages");
                await downloadPages();
                AnsiConsole.MarkupLine($"Downloaded {pagesDone.Count} Pages");

                cssToDo = cssToDo.Distinct().ToList();

                AnsiConsole.MarkupLine($"Downloading {cssToDo.Count} CSS Files");
                cssToDo.ForEach(async css => await downloadNextCSS(css));

                jsToDo = jsToDo.Distinct().ToList();

                AnsiConsole.MarkupLine($"Downloading {cssToDo.Count} JS Files");
                jsToDo.ForEach(async js => await downloadNextJS(js));

                imagesToDo = imagesToDo.Distinct().ToList();

                AnsiConsole.MarkupLine($"Downloading {cssToDo.Count} Image Files");
                imagesToDo.ForEach(async image => await downloadNextImage(image));
            });

            AnsiConsole.Markup("Your files are ready :rocket::");
            AnsiConsole.Write(new TextPath(fileAccessHandler.GetAbsoluteBasePath()));

            AnsiConsole.Write(new Rule("[blue]Nerd stats[/]"));

            int totalCount = pagesDone.Count() + jsToDo.Count() + imagesToDo.Count() + cssToDo.Count();

            AnsiConsole.Write(new BreakdownChart()
                .Width(totalCount)
                .AddItem("HTML", pagesDone.Count, Color.Blue)
                .AddItem("JS", jsToDo.Count, Color.Green)
                .AddItem("Images", imagesToDo.Count, Color.Blue)
                .AddItem("CSS", cssToDo.Count, Color.Aqua));

            AnsiConsole.Write(new Rule());
        }

        private async Task downloadPages()
        {
            while (pagesToDo.Count > 0)
            {
                string nextPageUrl = pagesToDo.First();

                if (pagesDone.Contains(nextPageUrl))
                {
                    pagesToDo.Remove(nextPageUrl);
                    continue;
                }

                await downloadNextPage(nextPageUrl);

                pagesToDo.Remove(nextPageUrl);
                pagesDone.Add(nextPageUrl);
            }
        }

        private async Task downloadNextPage(string nextUrlString)
        {
            Url url = new Url(nextUrlString);

            string html = await httpAccessHandler.RequestPageAsync(url);

            if (string.IsNullOrEmpty(html))
            {
                return;
            }

            string fileNameWithPath = "/index.html";

            if (url.ToString().EndsWith(".html"))
            {
                fileNameWithPath = url.Path;
            }
            else if (url.Path != "" && url.Path != "/")
            {
                fileNameWithPath = $"{url.Path}.html";
            }

            if (!fileAccessHandler.DoesFileAlreadyExists(fileNameWithPath))
                fileAccessHandler.SaveFile(Encoding.UTF8.GetBytes(html), fileNameWithPath);

            // Sites
            List<string> nextPages = NextPageParser.Parse(html, this.rootUrl);
            pagesToDo.AddRange(nextPages);
            pagesToDo = pagesToDo.Distinct().ToList();

            // CSS
            List<string> nextCSS = CssPageParser.Parse(html, this.rootUrl);
            cssToDo.AddRange(nextCSS);

            // JS
            List<string> nextJS = JsPageParser.Parse(html, this.rootUrl);
            jsToDo.AddRange(nextJS);

            // Image
            List<string> nextImage = ImagePageParser.Parse(html, this.rootUrl);
            imagesToDo.AddRange(nextImage);
        }

        private async Task downloadNextCSS(string nextCSSUrlString)
        {
            Url url = new Url(nextCSSUrlString);

            string css = await httpAccessHandler.RequestPageAsync(url);

            string fileNameWithPath = "";

            if (url.ToString().EndsWith(".css"))
            {
                fileNameWithPath = url.Path;
            }
            else
                return;
            if (!fileAccessHandler.DoesFileAlreadyExists(fileNameWithPath))
                fileAccessHandler.SaveFile(Encoding.UTF8.GetBytes(css), fileNameWithPath);
        }

        private async Task downloadNextJS(string nextJSUrlString)
        {
            Url url = new Url(nextJSUrlString);

            string css = await httpAccessHandler.RequestPageAsync(url);

            string fileNameWithPath = "";

            if (url.ToString().EndsWith(".js"))
            {
                fileNameWithPath = url.Path;
            }
            else
                return;
            if (!fileAccessHandler.DoesFileAlreadyExists(fileNameWithPath))
                fileAccessHandler.SaveFile(Encoding.UTF8.GetBytes(css), fileNameWithPath);
        }

        private async Task downloadNextImage(string nextImageUrlString)
        {
            Url url = new Url(nextImageUrlString);

            byte[] image = await httpAccessHandler.RequestFileAsync(url);

            string fileNameWithPath = "";

            if (commonImageTypes.Any(t => url.ToString().EndsWith(t)))
            {
                fileNameWithPath = url.Path;
            }
            else
                return;
            if (!fileAccessHandler.DoesFileAlreadyExists(fileNameWithPath))
                fileAccessHandler.SaveFile(image, fileNameWithPath);
        }
    }
}