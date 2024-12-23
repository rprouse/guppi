using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Entities.Strava;
using Spectre.Console;
using Guppi.Core.Interfaces.Services;

namespace Guppi.Console.Skills;

internal class StravaSkill : ISkill
{
    private readonly IStravaService _service;

    public StravaSkill(IStravaService service)
    {
        _service = service;
    }

    public IEnumerable<Command> GetCommands()
    {
        var view = new Command("view", "Views activities from the past week")
        {
            new Option<int>(new string[]{"--days", "-d" }, () => 7, "Number of days to view up to 90. Defaults to 7.")
        };

        view.Handler = CommandHandler.Create(async (int days) => await View(days));

        var configure = new Command("configure", "Configures the Strava provider");
        configure.AddAlias("config");
        configure.Handler = CommandHandler.Create(() => Configure());

        var command = new Command("strava", "Displays Strava fitness activities")
        {
           view,
           configure
        };
        command.AddAlias("fitness");
        return new[] { command };
    }

    private async Task View(int days)
    {
        try
        {
            IEnumerable<Activity> activities = await _service.GetActivities();

            AnsiConsoleHelper.TitleRule($":person_biking: Fitness activities from the last {days} days");

            var table = new Table();
            table.Border = TableBorder.Minimal;
            table.AddColumns("", "Day", ":calendar: Date", ":sports_medal: Distance", ":four_o_clock: Duration", ":mount_fuji: Elevaton", ":growing_heart: Suffer", ":compass: Activity");
            table.Columns[0].LeftAligned();
            table.Columns[1].LeftAligned();
            table.Columns[2].LeftAligned();
            table.Columns[3].RightAligned();
            table.Columns[4].RightAligned();
            table.Columns[5].RightAligned();
            table.Columns[6].RightAligned();
            table.Columns[7].LeftAligned();

            var lastWeek = DateTime.Now.AddDays(-days).Date;
            foreach(var act in activities.Where(a => a.StartDate >= lastWeek).OrderBy(a => a.StartDate))
            {
                table.AddRow(
                    act.Icon,
                    act.StartDate.ToLocalTime().ToString("ddd"),
                    act.StartDate.ToLocalTime().ToString("yyyy-MM-dd"),
                    $"{(act.Distance / 1000):0.0} km",
                    act.MovingTime.ToString(@"hh\:mm\:ss"),
                    $"{act.Elevation:0} m",
                    (act.SufferScore ?? 0).ToString(),
                    act.Name);
            }
            AnsiConsole.Write(table);

            AnsiConsoleHelper.Rule("white");
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
        }
        catch (UnauthorizedException ue)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
        }
    }

    private void Configure() => _service.Configure();
}
