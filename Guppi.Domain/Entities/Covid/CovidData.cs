using System;
using System.Collections.Generic;

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

        public double CasesPerHundredThousand =>
            LatestCases * 100000d / Population;

        public double DeathsPerHundredThousand =>
            LatestDeaths * 100000d / Population;
    }
}
