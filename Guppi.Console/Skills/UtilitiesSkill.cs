using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Guppi.Core.Interfaces.Services;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class UtilitiesSkill : ISkill
{
    private readonly IUtilitiesService _service;

    public UtilitiesSkill(IUtilitiesService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        var time = new Command("time", "Displays the current date/time")
        {
            new Option<bool>(new string[]{"--utc", "-u"}, "Displays the time as UTC")
        };
        time.Handler = CommandHandler.Create((bool utc) => DisplayTime(utc));
        yield return time;

        var date = new Command("date", "Displays the current date")
        {
            new Option<bool>(new string[]{"--utc", "-u"}, "Displays the date as UTC")
        };
        date.Handler = CommandHandler.Create((bool utc) => DisplayDate(utc));
        yield return date;

        var guid = new Command("guid", "Creates a new Guid");
        guid.Handler = CommandHandler.Create(() => NewGuid());
        yield return guid;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var explorer = new Command("reset", "Restarts Windows Explorer");
            explorer.Handler = CommandHandler.Create(async () => await RestartExplorer());
            yield return explorer;
        }
    }

    void DisplayTime(bool utc)
    {
        var now = utc ? DateTime.UtcNow : DateTime.Now;
        AnsiConsole.MarkupLine($":two_o_clock: [silver][[{now:yyyy-MM-dd.}[/][white]{now:HH:mm:ss.fffK}]][/]");
        AnsiConsole.WriteLine();
    }

    void DisplayDate(bool utc)
    {
        var now = utc ? DateTime.UtcNow : DateTime.Now;
        AnsiConsole.MarkupLine($":tear_off_calendar: [silver][[{now:yyyy-MM-dd}]][/]");
        AnsiConsole.WriteLine();
    }

    void NewGuid()
    {
        AnsiConsole.MarkupLine($"[silver][[{Guid.NewGuid():D}]][/]");
        AnsiConsole.WriteLine();
    }

    async Task RestartExplorer()
    {
        try
        {
            AnsiConsole.MarkupLine(":firecracker: Restarting Explorer.exe");
            await _service.RestartExplorer();
        }
        catch (Exception ex)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: {ex.Message}]][/]");
        }
    }
}
