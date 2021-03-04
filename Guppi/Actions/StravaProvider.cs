using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Application.Commands.Strava;
using Guppi.Application.Exceptions;
using Guppi.Application.Extensions;
using Guppi.Domain.Entities.Weather;
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
            var view = new Command("view", "Views activities from the past week");

            view.Handler = CommandHandler.Create(async (bool all) => await Execute(all));

            var configure = new Command("configure", "Configures the weather provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(async () => await Configure());

            return new Command("strava", "Displays today's weather")
            {
               view,
               configure
            };
        }

        private async Task Execute(bool all)
        {
            try
            {
                //WeatherForecast weather = await _mediator.Send(new WeatherQuery());

                AnsiConsoleHelper.Rule("white");
            }
            catch (UnconfiguredException ue)
            {
                AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: {ue.Message}]][/]");
            }
        }

        private async Task Configure() => await _mediator.Send(new ConfigureStravaCommand());
    }
}
