using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Guppi.Infrastructure.Services.AdventOfCode
{
#pragma warning disable IDE1006 // Naming Styles
    internal class Leaderboard
    {
        public string year { get; set; }
        public string owner_id { get; set; }

        public Dictionary<string, Member> members { get; set; }

        public Guppi.Domain.Entities.AdventOfCode.Leaderboard GetLeaderboard() =>
            new Domain.Entities.AdventOfCode.Leaderboard
            {
                Year = year,
                OwnerId = owner_id,
                Members = members.ToDictionary(m => m.Key, m => m.Value.GetMember())
            };
    }

    internal class Member
    {
        public string name { get; set; }
        public string id { get; set; }
        public int global_score { get; set; }
        public int local_score { get; set; }
        public int stars { get; set; }
        public Dictionary<string, Day> completion_day_level { get; set; }

        public Guppi.Domain.Entities.AdventOfCode.Member GetMember() =>
            new Guppi.Domain.Entities.AdventOfCode.Member
            {
                Name = name,
                Id = id,
                GlobalScore = global_score,
                LocalScore = local_score,
                Stars = stars,
                CompletionDayLevel = completion_day_level.ToDictionary(c => c.Key, c => c.Value.GetDay())
            };
    }

    internal class Part
    {
        public string get_star_ts { get; set; }
    }

    internal class Day
    {
        [JsonPropertyName("1")]
        public Part PartOne { get; set; }
        [JsonPropertyName("2")]
        public Part PartTwo { get; set; }

        public Guppi.Domain.Entities.AdventOfCode.Day GetDay() =>
            new Guppi.Domain.Entities.AdventOfCode.Day
            {
                PartOneComplete = PartOne != null,
                PartTwoComplete = PartTwo != null
            };
    }
#pragma warning restore IDE1006 // Naming Styles
}
