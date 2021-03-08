using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Application.Extensions;
using Guppi.Application.Queries.Covid;
using Guppi.Domain.Common;
using Guppi.Domain.Entities.Covid;
using MediatR;
using Spectre.Console;

namespace Guppi.Console.Actions
{
    public class CovidProvider : IActionProvider
    {
        private readonly IMediator _mediator;

        public CovidProvider(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Command GetCommand()
        {
            var view = new Command("view", "Views Covid-19 Stats for a country. Defaults to Canada.")
            {
                new Option<string>(new string[]{"--country", "-c" }, () => "Canada", "The country to view. Defaults to Canada. Valid countries, Canada, Brazil, Germany, Spain, France, UK, India, Italy, Mexico and USA")
            };
            view.Handler = CommandHandler.Create(async (string country) => await View(country));

            return new Command("covid", "Displays Covid-19 Stats")
            {
               view
            };
        }

        private async Task View(string country)
        {
            try
            {
                var c = Countries.GetCountry(country);
                if(c == Country.Unknown)
                {
                    AnsiConsole.MarkupLine($"[red][[:cross_mark: Could not find country ${country}]][/]");
                    return;
                }

                CovidData data = await _mediator.Send(new CovidCountryDataQuery { Country = c });

                AnsiConsoleHelper.TitleRule($":biohazard: Covid data for {country} days");

                AnsiConsole.MarkupLine($"[white]Cases:  [/][silver]{data.RegionData.LatestCases,8:n0} {data.RegionData.CasesPerHundredThousand,7:n0}/100k[/]");
                AnsiConsole.MarkupLine($"[white]Deaths: [/][silver]{data.RegionData.LatestDeaths,8:n0} {data.RegionData.DeathsPerHundredThousand,7:n0}/100k[/]");

                AnsiConsole.WriteLine();

                var table = new Table();
                table.AddColumns("", "Cases", "Per 100k", "7D Avg", "Per 100k");
                table.Columns[0].Alignment(Justify.Left);
                table.Columns[1].Alignment(Justify.Right);
                table.Columns[2].Alignment(Justify.Right);
                table.Columns[3].Alignment(Justify.Right);
                foreach (var region in data.SubRegionData.Where(d => d.Name != "Unknown").OrderByDescending(d => d.CasesPerHundredThousand))
                {
                    table.AddRow(
                        region.Name, 
                        region.LatestCases.ToString("n0"), 
                        region.CasesPerHundredThousand.ToString("n0"), 
                        region.DailyAverageCasesLastSevenDays.ToString("n0"), 
                        "");
                }
                AnsiConsole.Render(table);
            }
            catch (Exception ue)
            {
                AnsiConsole.MarkupLine($"[red][[:cross_mark: ${ue.Message}]][/]");
            }
        }
    }
}
