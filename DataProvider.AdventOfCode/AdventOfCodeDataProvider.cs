using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using ColoredConsole;
using Guppi.Core;

namespace DataProvider.AdventOfCode
{
    public class AdventOfCodeDataProvider : IDataProvider
    {
        AdventOfCodeConfiguration _configuration;

        public AdventOfCodeDataProvider()
        {
            _configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        }

        public Command GetCommand()
        {
            var view = new Command("view", "Views the Advent of Code leaderboard")
            {
                new Option<int>(new string[]{"--year", "-y" }, () => DateTime.Now.Year, "Displays the leaderboard for the given year. Defaults to this year."),
                new Option<int>(new string[]{"--board", "-b" }, () => 274125, "The id of the leaderboard you want to view. Defaults to the CoderCamp leaderboard")
            };

            view.Handler = CommandHandler.Create(async (int year, int board) => await ViewLeaderboard(year, board));

            var configure = new Command("configure", "Configures the AdventOfCode provider");
            configure.AddAlias("config");
            configure.Handler = CommandHandler.Create(() => Configure());

            return new Command("aoc", "Work with Advent of Code (AoC)")
            {
               view,
               configure
            };
        }

        private async Task ViewLeaderboard(int year, int board)
        {
            if (!_configuration.Configured)
            {
                ColorConsole.WriteLine("Please configure the Advent of Code provider".Yellow());
                return;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/myday)");
            client.DefaultRequestHeaders.Add("Cookie", $"session={_configuration.LoginToken}");
            string json = await client.GetStringAsync($"https://adventofcode.com/{year}/leaderboard/private/view/{board}.json");

            var leaders = JsonSerializer.Deserialize<Leaderboard>(json);

            ColorConsole.WriteLine($"=== CodeCamp Leaderboard {year} ===".Yellow());
            ColorConsole.WriteLine();

            int place = 1;
            foreach (var member in leaders.members.Values.OrderByDescending(m => m.local_score).ThenByDescending(m => m.stars))
            {
                ColorConsole.Write(string.Format("{0,3}) {1,3}  ", place++, member.local_score).White());

                for (int d = 1; d <= 25; d++)
                {
                    if (DateTime.Now.Date < new DateTime(year, 12, d))
                    {
                        ColorConsole.Write("·".DarkGray());
                        continue;
                    }

                    string day = d.ToString();
                    ColorToken star;
                    if (member.completion_day_level.ContainsKey(day))
                    {
                        Day completed = member.completion_day_level[day];
                        if (completed.PartOne != null && completed.PartTwo != null)
                            star = "*".Yellow();
                        else
                            star = "*".White();
                    }
                    else
                    {
                        star = "*".DarkGray();
                    }
                    ColorConsole.Write(star);
                }
                ColorConsole.WriteLine($"  {member.name}".Green());
            }
        }

        private void Configure()
        {
            _configuration.RunConfiguration("Advent of Code", "Enter the Advent of Code session cookie value.");
        }
    }

#pragma warning disable IDE1006 // Naming Styles
    public class Leaderboard
    {
        public string year { get; set; }
        public string owner_id { get; set; }

        public Dictionary<string, Member> members { get; set; }
    }

    public class Member
    {
        public string name { get; set; }
        public string id { get; set; }
        public int global_score { get; set; }
        public int local_score { get; set; }
        public int stars { get; set; }
        public Dictionary<string, Day> completion_day_level { get; set; }
    }

    public class Part
    {
        public string get_star_ts { get; set; }
    }

    public class Day
    {
        [JsonPropertyName("1")]
        public Part PartOne { get; set; }
        [JsonPropertyName("2")]
        public Part PartTwo { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles

}
