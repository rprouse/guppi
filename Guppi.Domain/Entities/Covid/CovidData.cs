using System;
using System.Collections.Generic;
using System.Linq;

namespace Guppi.Domain.Entities.Covid
{
    public class CovidData
    {
        public RegionData RegionData { get; init; }
        public List<RegionData> SubRegionData { get; init; }
    }

    public class RegionData
    {
        public string Name { get; init; }
        public string GeoId { get; init; }
        public string Link { get; init; }
        public string RegionType { get; init; }
        public int Population { get; init; }
        public int LatestCases { get; init; }
        public int LatestDeaths { get; init; }
        public Dictionary<DateTime, int> Cases { get; } = new Dictionary<DateTime, int>();
        public Dictionary<DateTime, int> Deaths { get; } = new Dictionary<DateTime, int>();

        public int CasesPerHundredThousand =>
            (int)(LatestCases * 100000d / Population);

        public int DeathsPerHundredThousand =>
            (int)(LatestDeaths * 100000d / Population);

        public int DailyAverageCasesLastSevenDays
        {
            get
            {
                var latestDate = Cases.Keys.Max();
                var prevWeek = latestDate.AddDays(-7);
                int prev = Cases[prevWeek];
                int current = Cases[latestDate];
                return (current - prev) / 7;
            }
        }
    }
}
