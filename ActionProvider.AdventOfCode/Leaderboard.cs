using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ActionProvider.AdventOfCode
{
#pragma warning disable IDE1006 // Naming Styles
    internal class Leaderboard
    {
        public string year { get; set; }
        public string owner_id { get; set; }

        public Dictionary<string, Member> members { get; set; }
    }

    internal class Member
    {
        public string name { get; set; }
        public string id { get; set; }
        public int global_score { get; set; }
        public int local_score { get; set; }
        public int stars { get; set; }
        public Dictionary<string, Day> completion_day_level { get; set; }
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
    }
#pragma warning restore IDE1006 // Naming Styles
}
