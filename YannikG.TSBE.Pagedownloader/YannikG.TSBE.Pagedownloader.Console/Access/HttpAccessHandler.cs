using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;
using Spectre.Console;

namespace YannikG.TSBE.Pagedownloader.Console.Access
{
    public class HttpAccessHandler
    {
        private readonly string baseUrl;
        private readonly FlurlClient cli;

        public HttpAccessHandler(string baseUrl)
        {
            this.baseUrl = baseUrl;
            this.cli = new FlurlClient();
        }

        public async Task<string> RequestPageAsync(string url, string? path = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url cannot be null or empty");

            if (string.IsNullOrEmpty(path) && !url.Contains(baseUrl))
                throw new ArgumentException("path cannot be null or empty when url does not contain the baseUrl");

            if (!string.IsNullOrEmpty(path))
                url = (baseUrl + path).Replace("//", "/");

            try
            {
                var httpResult = await cli.Request(url).GetAsync();

                if (httpResult.ResponseMessage.IsSuccessStatusCode)
                {
                    return await httpResult.GetStringAsync();
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[orange3] expected exception[/], program will [green]continue[/]:");
                AnsiConsole.WriteException(ex);

                return string.Empty;
            }
        }

        public async Task<byte[]> RequestFileAsync(string url, string? path = null)
        {
            if (string.IsNullOrEmpty(url))
                throw new ArgumentNullException("url cannot be null or empty");

            if (string.IsNullOrEmpty(path) && !url.Contains(baseUrl))
                throw new ArgumentException("path cannot be null or empty when url does not contain the baseUrl");

            if (!string.IsNullOrEmpty(path))
                url = (baseUrl + path).Replace("//", "/");

            try
            {
                var httpResult = await cli.Request(url).GetAsync();

                if (httpResult.ResponseMessage.IsSuccessStatusCode)
                {
                    return await httpResult.GetBytesAsync();
                }
                else
                {
                    return new List<byte>().ToArray();
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine("[orange3] expected exception[/], program will [green]continue[/]:");
                AnsiConsole.WriteException(ex);

                return new List<byte>().ToArray();
            }
        }
    }
}