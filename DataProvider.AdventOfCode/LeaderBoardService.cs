using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using ColoredConsole;

namespace DataProvider.AdventOfCode
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
                ColorConsole.WriteLine("[Please configure the Advent of Code provider]".Yellow());
                return;
            }

            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/myday)");
            client.DefaultRequestHeaders.Add("Cookie", $"session={_configuration.LoginToken}");
            string json = await client.GetStringAsync($"https://adventofcode.com/{year}/leaderboard/private/view/{board}.json");

            var leaders = JsonSerializer.Deserialize<Leaderboard>(json);

            ColorConsole.WriteLine();
            ColorConsole.WriteLine($"=== Advent of Code Leaderboard {year} ===".Yellow());
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
    }
}
