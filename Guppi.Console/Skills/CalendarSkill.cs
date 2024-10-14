using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces;
using Guppi.Core.Services.Dictionary;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class CalendarSkill(ICalendarService service) : ISkill
{
    private readonly ICalendarService _service = service;

    public IEnumerable<Command> GetCommands()
    {
        var markdown = new Option<bool>(new string[] { "--markdown", "-m" }, "Display as Markdown to be copied into Notes");
        var next = new Command("next", "Views next calendar event") { markdown };
        next.AddAlias("view");
        next.SetHandler(async (bool markdown) => await Next(markdown), markdown);

        var today = new Command("today", "Displays today's agenda") { markdown };
        today.AddAlias("agenda");
        today.SetHandler(async (bool markdown) =>
        {
            var now = DateTime.Now;
            var midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
            await Agenda(midnight, "Today's agenda", markdown);
        }, markdown);

        var tomorrow = new Command("tomorrow", "Displays tomorrow's agenda") { markdown };
        tomorrow.SetHandler(async (bool markdown) =>
        {
            var now = DateTime.Now.AddDays(1);
            var midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
            await Agenda(midnight, "Tomorrow's agenda", markdown);
        }, markdown);

        var free = new Command("free", "Displays free time for a given day");
        free.AddArgument(new Argument<DateTime>("date", "The date to check"));
        free.Handler = CommandHandler.Create<DateTime>(FreeTime);

        var logout = new Command("logout", "Logs out of the current Google account");
        logout.SetHandler(async () => await Logout());

        var configure = new Command("Configure", "Configure calendars");
        configure.AddAlias("config");
        configure.SetHandler(() => Configure());

        var cmd = new Command("calendar", "Displays today's calendar events")
        {
            next,
            today,
            tomorrow,
            free,
            logout,
            configure
        };
        cmd.AddAlias("cal");
        return new[] { cmd };
    }

    private void Configure() => _service.Configure();

    private async Task Logout()
    {
        await _service.Logout();
        AnsiConsole.MarkupLine("[green][[:check_mark_button: Logged out of Google]][/]");
    }

    private async Task Next(bool markdown)
    {
        try
        {
            var now = DateTime.Now;
            var events = await _service.GetCalendarEvents(now, now.AddDays(7));
            AnsiConsoleHelper.TitleRule(":tear_off_calendar: Next event");
            StringBuilder sb = new();

            try
            {
                if (events.Any())
                {
                    foreach (var eventItem in events)
                    {
                        string start = eventItem.Start?.ToString("MMM dd HH:mm");
                        if (string.IsNullOrEmpty(start))
                        {
                            continue;
                        }
                        string end = eventItem.End?.ToString("-HH:mm") ?? "";
                        if (markdown)
                        {
                            var line = $"- **{start}{end}** {eventItem.Summary}{JoinLink(eventItem)}";
                            sb.AppendLine(line);
                            AnsiConsole.WriteLine(line);
                            TextCopy.ClipboardService.SetText(sb.ToString());
                            AnsiConsole.MarkupLine("[green]:green_circle: Copied to clipboard[/]");
                            AnsiConsole.WriteLine();
                        }
                        else
                            AnsiConsole.MarkupLine($"{eventItem.Start.GetEmoji()} [cyan]{start}{end}\t[/][silver]{eventItem.Summary}[/]");
                        return;
                    }
                }
                AnsiConsole.MarkupLine("[white][[No upcoming events found.]][/]");
            }
            finally
            {
                AnsiConsoleHelper.Rule("white");
            }
        }
        catch (UnauthorizedException ue)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
            return;
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: ${ue.Message}]][/]");
        }
    }

    private async Task Agenda(DateTime now, string title, bool markdown)
    {
        try
        {
            var max = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, DateTimeKind.Local);
            var events = await _service.GetCalendarEvents(now, max);
            AnsiConsoleHelper.TitleRule($":calendar: {title}");
            StringBuilder sb = new();

            try
            {
                if (events.Any())
                {
                    bool found = false;
                    foreach (var _ in events.Where(eventItem => DisplayEvent(eventItem, markdown, sb)).Select(eventItem => new { }))
                    {
                        found = true;
                    }

                    if (found)
                    {
                        if (markdown)
                        {
                            TextCopy.ClipboardService.SetText(sb.ToString());
                            AnsiConsole.WriteLine();
                            AnsiConsole.MarkupLine("[green]:green_circle: Copied to clipboard[/]");
                        }
                        return;
                    }
                }
                AnsiConsole.MarkupLine("[white][[No upcoming events found.]][/]");
            }
            finally
            {
                AnsiConsole.WriteLine();
                AnsiConsoleHelper.Rule("white");
            }
        }
        catch (UnauthorizedException ue)
        {
            AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
            return;
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: ${ue.Message}]][/]");
        }
    }

    private static bool DisplayEvent(Core.Entities.Calendar.Event eventItem, bool markdown, StringBuilder markdownBuffer)
    {
        string start = eventItem.Start?.ToString("HH:mm");
        if (string.IsNullOrEmpty(start))
        {
            return false;
        }
        string end = eventItem.End?.ToString("-HH:mm") ?? "";
        if (markdown)
        {
            var line = $"- {eventItem.Start.GetEmoji()} **{start}{end}** {eventItem.Summary}{JoinLink(eventItem)}";
            markdownBuffer.AppendLine(line);
            AnsiConsole.WriteLine(line);
        }
        else
            AnsiConsole.MarkupLine($"{eventItem.Start.GetEmoji()} [cyan]{start}{end}\t[/][silver]{eventItem.Summary}[/]");

        return true;
    }

    private async Task FreeTime(DateTime date)
    {
        var start = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, DateTimeKind.Local);
        var end = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59, DateTimeKind.Local);
        var events = await _service.GetCalendarEvents(start, end);
        AnsiConsoleHelper.TitleRule($":calendar: Free time for {date:MMM dd}");
        try
        {
            if (events.Any())
            {
                var freeTime = new List<(DateTime? start, DateTime? end)>();
                // Start at 09:00
                DateTime? lastEnd = new DateTime(date.Year, date.Month, date.Day, 9, 0, 0, DateTimeKind.Local);
                // End at 17:00
                end = new DateTime(date.Year, date.Month, date.Day, 17, 0, 0, DateTimeKind.Local);
                foreach (var eventItem in events)
                {
                    if (eventItem.Start > lastEnd && eventItem.Start.Value.Subtract(lastEnd.Value).TotalMinutes >= 30)
                    {
                        freeTime.Add((lastEnd, eventItem.Start));
                    }
                    lastEnd = eventItem.End;
                }
                if (lastEnd < end && end.Subtract(lastEnd.Value).TotalMinutes >= 30)
                {
                    freeTime.Add((lastEnd, end));
                }
                if (freeTime.Count > 0)
                {
                    foreach (var (st, en) in freeTime)
                    {
                        AnsiConsole.MarkupLine($"[silver]{st:HH:mm} - {en:HH:mm}[/]");
                    }
                    return;
                }
            }
            AnsiConsole.MarkupLine("[yellow][[No free time found.]][/]");
        }
        finally
        {
            AnsiConsoleHelper.Rule("white");
        }
    }

    private static string JoinLink(Core.Entities.Calendar.Event eventItem) =>
        string.IsNullOrEmpty(eventItem.MeetingUrl) ? "" : $" [Join]({eventItem.MeetingUrl})";
}
