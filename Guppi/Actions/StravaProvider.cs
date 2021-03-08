using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Commands.Strava;
using Guppi.Application.Exceptions;
using Guppi.Application.Extensions;
using Guppi.Application.Queries.Strava;
using Guppi.Domain.Entities.Strava;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Actions
{
    public class StravaProvider : IActionProvider
    {
        private readonly IMediator _mediator;

        public StravaProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Command GetCommand()
        {
            var view = new Command("view", "Views activities from the past week")
            {
                new Option<int>(new string[]{"--days", "-d" }, () => 7, "Number of days to view up to 90. Defaults to 7.")
            };

            view.Handler = CommandHandler.Create(async (int days) => await View(days));

            var configure = new Command("configure", "Configures the Strava provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(async () => await Configure());

            var command = new Command("strava", "Displays Strava fitness activities")
            {
               view,
               configure
            };
            command.AddAlias("fitness");
            return command;
        }

        private async Task View(int days)
        {
            try
            {
                IEnumerable<StravaActivity> activities = await _mediator.Send(new GetActivitiesQuery());

                AnsiConsoleHelper.TitleRule($":person_biking: Fitness activities from the last {days} days");

                var lastWeek = DateTime.Now.AddDays(-days).Date;
                foreach(var act in activities.Where(a => a.StartDate >= lastWeek).OrderBy(a => a.StartDate))
                {
                    AnsiConsole.MarkupLine($"{act.Icon} {act.StartDate:ddd yyyy-MM-dd} {(act.Distance / 1000),5:0.0} km :four_o_clock: {act.MovingTime:hh\\:mm\\:ss} :mount_fuji: {act.Elevation,4:0} m - {act.Name}");
                }

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

        private async Task Configure() => await _mediator.Send(new ConfigureStravaCommand());
    }
}
