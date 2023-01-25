using Spectre.Console;
using YannikG.TSBE.Pagedownloader.Console;

AnsiConsole.Write(
    new FigletText("The .NET 6.0 Pagedownloader")
        .LeftJustified()
        .Color(Color.Green));

AnsiConsole.WriteLine("Welcome!");

bool shouldContinue = true;

const string BASE_EXPORTPATH = "Exports";

do
{
    string url = AnsiConsole.Ask<string>("Enter a valid url (e.g 'http://mywebsite.com'");
    string exportName = AnsiConsole.Ask<string>("Enter a valid url (e.g 'mywebsite'");

    string finalExportPath = Path.Combine(BASE_EXPORTPATH, exportName);
    var downloadService = new DownloaderService(url, finalExportPath);
    try
    {
        await downloadService.StartAsync();
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine("[red] FATAL ERROR[/]:");
        AnsiConsole.WriteException(ex);
    }

    shouldContinue = AnsiConsole.Confirm("should we go for another round??");
    AnsiConsole.Clear();
} while (shouldContinue);

AnsiConsole.Write(
new FigletText("Good bye!")
    .Centered()
    .Color(Color.Green));