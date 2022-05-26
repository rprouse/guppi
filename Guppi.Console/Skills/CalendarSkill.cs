using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Commands.Calendar;
using Guppi.Application.Exceptions;
using Guppi.Application.Extensions;
using Guppi.Application.Queries.Calendar;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Skills
{
    public class CalendarSkill : ISkill
    {
        private readonly IMediator _mediator;

        public CalendarSkill(IMediator mediator)
        {
            _mediator = mediator;
        }

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

            var logout = new Command("logout", "Logs out of the current Google account");
            logout.SetHandler(async () => await Logout());

            var configure = new Command("Configure", "Configure calendars");
            configure.AddAlias("config");
            configure.SetHandler(async () => await Configure());

            var cmd = new Command("calendar", "Display's today's calendar events")
            {
                next,
                today,
                tomorrow,
                logout,
                configure
            };
            cmd.AddAlias("cal");
            return new[] { cmd };
        }

        private async Task Configure() =>
            await _mediator.Send(new ConfigureCalendarCommand());

        private async Task Logout()
        {
            await _mediator.Send(new CalendarLogoutCommand());
            AnsiConsole.MarkupLine("[green][[:check_mark_button: Logged out of Google]][/]");
        }

        private async Task Next(bool markdown)
        {
            var now = DateTime.Now;

            var query = new CalendarEventsQuery
            {
                MinDate = now,
                MaxDate = now.AddDays(7)
            };

            try
            {
                var events = await _mediator.Send(query);
                AnsiConsoleHelper.TitleRule(":tear_off_calendar: Next event");

                try
                {
                    if (events.Count() > 0)
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
                                AnsiConsole.WriteLine($"- **{start}{end}** {eventItem.Summary}");
                            else
                                AnsiConsole.MarkupLine($"{eventItem.Start.GetEmoji()} [white]{start}{end}\t[/][silver]{eventItem.Summary}[/]");
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
            catch(UnauthorizedException ue)
            {
                AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
                return;
            }
            catch(UnconfiguredException ue)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: ${ue.Message}]][/]");
            }
        }

        private async Task Agenda(DateTime now, string title, bool markdown)
        {
            var query = new CalendarEventsQuery
            {
                MinDate = now,
                MaxDate = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59, DateTimeKind.Local)
            };

            try
            {
                var events = await _mediator.Send(query);
                AnsiConsoleHelper.TitleRule($":calendar: {title}");

                try
                {
                    if (events.Count() > 0)
                    {
                        bool found = false;
                        foreach (var eventItem in events)
                        {
                            string start = eventItem.Start?.ToString("HH:mm");
                            if (string.IsNullOrEmpty(start))
                            {
                                continue;
                            }
                            string end = eventItem.End?.ToString("-HH:mm") ?? "";
                            if (markdown)
                                AnsiConsole.WriteLine($"- {eventItem.Start.GetEmoji()} **{start}{end}** {eventItem.Summary}");
                            else
                                AnsiConsole.MarkupLine($"{eventItem.Start.GetEmoji()} [white]{start}{end}\t[/][silver]{eventItem.Summary}[/]");
                            found = true;
                        }
                        if (found) return;
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
    }
}
