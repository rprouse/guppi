using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Application.Extensions;
using Spectre.Console;

namespace ActionProvider.AdventOfCode
{
    internal class LeaderBoardService
    {
        AdventOfCodeConfiguration _configuration;

        public LeaderBoardService(AdventOfCodeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task ViewLeaderboard(int year, int board)
        {
            if (!_configuration.Configured)
            {
                AnsiConsole.MarkupLine("[yellow][[:yellow_circle: Please configure the Advent of Code provider]][/]");
                return;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/myday)");
            client.DefaultRequestHeaders.Add("Cookie", $"session={_configuration.LoginToken}");
            string json = await client.GetStringAsync($"https://adventofcode.com/{year}/leaderboard/private/view/{board}.json");

            var leaders = JsonSerializer.Deserialize<Leaderboard>(json);

            AnsiConsole.WriteLine();
            AnsiConsoleHelper.TitleRule($":christmas_tree: Advent of Code Leaderboard {year}");

            int place = 1;
            foreach (var member in leaders.members.Values.OrderByDescending(m => m.local_score).ThenByDescending(m => m.stars))
            {
                AnsiConsole.Markup(string.Format("[white]{0,3}) {1,3}[/]  ", place++, member.local_score));

                for (int d = 1; d <= 25; d++)
                {
                    if (DateTime.Now.Date < new DateTime(year, 12, d))
                    {
                        AnsiConsole.Markup("[grey]·[/]");
                        continue;
                    }

                    string day = d.ToString();
                    string star;
                    if (member.completion_day_level.ContainsKey(day))
                    {
                        Day completed = member.completion_day_level[day];
                        if (completed.PartOne != null && completed.PartTwo != null)
                            star = "[yellow]*[/]";
                        else
                            star = "[white]*[/]";
                    }
                    else
                    {
                        star = "[grey]*[/]";
                    }
                    AnsiConsole.Markup(star);
                }
                AnsiConsole.MarkupLine($"[green]  {member.name}[/]");
            }
            AnsiConsoleHelper.Rule("white");
        }
    }
}
