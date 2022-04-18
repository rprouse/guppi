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
            var view = new Command("view", "Views upcoming calendar events")
            {
                new Option<bool>(new string[]{"--agenda", "-a" }, "Displays today's agenda")
            };
            view.Handler = CommandHandler.Create(async (bool agenda) => await Execute(agenda));

            var logout = new Command("logout", "Logs out of the current Google account");
            logout.Handler = CommandHandler.Create(async () => await Logout());

            var cmd = new Command("calendar", "Display's today's calendar events")
            {
                view,
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

        private async Task Execute(bool agenda)
        {
            var now = DateTime.Now;

            var query = new CalendarEventsQuery
            {
                MinDate = now,
                MaxDate = agenda ? new DateTime(now.Year, now.Month, now.Day, 23, 59, 59) : null
            };

            try
            {
                var events = await _mediator.Send(query);

                string title = agenda ? ":calendar: Today's agenda" : ":tear_off_calendar: Next event";

                AnsiConsoleHelper.TitleRule(title);

                try
                {
                    if (events.Count() > 0)
                    {
                        bool found = false;
                        foreach (var eventItem in events)
                        {
                            string start = eventItem.Start?.ToString(agenda ? "HH:mm" : "MMM dd HH:mm");
                            if (string.IsNullOrEmpty(start))
                            {
                                continue;
                            }
                            string end = eventItem.End?.ToString("-HH:mm") ?? "";
                            AnsiConsole.MarkupLine($"{eventItem.Start.GetEmoji()} [white]{start}{end}\t[/][silver]{eventItem.Summary}[/]");
                            found = true;
                            if (!agenda) return;
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
    }
}
