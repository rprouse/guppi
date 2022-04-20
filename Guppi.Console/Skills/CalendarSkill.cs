using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
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
            var next = new Command("next", "Views next calendar event");
            next.AddAlias("view");
            next.Handler = CommandHandler.Create(async () => await Next());

            var today = new Command("today", "Displays today's agenda");
            today.AddAlias("agenda");
            today.Handler = CommandHandler.Create(async () => await Agenda(DateTime.Now, "Today's agenda"));

            var tomorrow = new Command("tomorrow", "Displays tomorrow's agenda");
            tomorrow.Handler = CommandHandler.Create(async () => 
            {
                var now = DateTime.Now.AddDays(1);
                var midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);
                await Agenda(midnight, "Tomorrow's agenda");
            });

            var logout = new Command("logout", "Logs out of the current Google account");
            logout.Handler = CommandHandler.Create(async () => await Logout());

            var cmd = new Command("calendar", "Display's today's calendar events")
            {
                next,
                today,
                tomorrow,
                logout
            };
            cmd.AddAlias("cal");
            return new[] { cmd };
        }

        private async Task Logout()
        {
            await _mediator.Send(new CalendarLogoutCommand());
            AnsiConsole.MarkupLine("[green][[:check_mark_button: Logged out of Google]][/]");
        }

        private async Task Next()
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

        private async Task Agenda(DateTime now, string title)
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
