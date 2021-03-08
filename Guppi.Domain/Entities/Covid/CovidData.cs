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

        public DateTime LatestDate { get; init; }

        public double CasesPerHundredThousand =>
            LatestCases * 100000d / Population;

        public double DeathsPerHundredThousand =>
            LatestDeaths * 100000d / Population;

        int? _dailyAverageCasesLastSevenDays;

        public int DailyAverageCasesLastSevenDays
        {
            get
            {
                if (!_dailyAverageCasesLastSevenDays.HasValue)
                {
                    if (Cases.Count > 0)
                    {
                        var latestDate = Cases.Keys.Max();
                        var prevWeek = latestDate.AddDays(-7);
                        int prev = Cases[prevWeek];
                        int current = Cases[latestDate];
                        _dailyAverageCasesLastSevenDays = (current - prev) / 7;
                    }
                    else
                    {
                        _dailyAverageCasesLastSevenDays = 0;
                    }
                }
                return _dailyAverageCasesLastSevenDays.Value;
            }
        }

        public double DailyAverageCasesLastSevenDaysPerHundredThousand =>
            DailyAverageCasesLastSevenDays * 100000d / Population;

        int? _dailyAverageDeathsLastSevenDays;

        public int DailyAverageDeathsLastSevenDays
        {
            get
            {
                if (!_dailyAverageDeathsLastSevenDays.HasValue)
                {
                    if (Deaths.Count > 0)
                    {
                        var lastDay = Deaths.Keys.Max();
                        var prevWeek = lastDay.AddDays(-7);
                        int prev = Deaths[prevWeek];
                        int current = Deaths[lastDay];
                        _dailyAverageDeathsLastSevenDays = (current - prev) / 7;
                    }
                    else
                    {
                        _dailyAverageDeathsLastSevenDays = 0;
                    }
                }
                return _dailyAverageDeathsLastSevenDays.Value;
            }
        }

        public double DailyAverageDeathsLastSevenDaysPerHundredThousand =>
            DailyAverageDeathsLastSevenDays * 100000d / Population;


        int? _dailyAverageCasesPreviousSevenDays;

        public int DailyAverageCasesPreviousSevenDays
        {
            get
            {
                if (!_dailyAverageCasesPreviousSevenDays.HasValue)
                {
                    if (Cases.Count > 0)
                    {
                        var latestDate = Cases.Keys.Max().AddDays(-7);
                        var prevWeek = latestDate.AddDays(-14);
                        int prev = Cases[prevWeek];
                        int current = Cases[latestDate];
                        _dailyAverageCasesPreviousSevenDays = (current - prev) / 7;
                    }
                    else
                    {
                        _dailyAverageCasesPreviousSevenDays = 0;
                    }    
                }
                return _dailyAverageCasesPreviousSevenDays.Value;
            }
        }

        int? _dailyAverageDeathsPreviousSevenDays;

        public int DailyAverageDeathsPreviousSevenDays
        {
            get
            {
                if (!_dailyAverageDeathsPreviousSevenDays.HasValue)
                {
                    if (Deaths.Count > 0)
                    {
                        var lastDay = Deaths.Keys.Max().AddDays(-7);
                        var prevWeek = lastDay.AddDays(-14);
                        int prev = Deaths[prevWeek];
                        int current = Deaths[lastDay];
                        _dailyAverageDeathsPreviousSevenDays = (current - prev) / 7;
                    }
                    else
                    {
                        _dailyAverageDeathsPreviousSevenDays = 0;
                    }
                }
                return _dailyAverageDeathsPreviousSevenDays.Value;
            }
        }

        int? _lastReportedCases;

        public int LastReportedCases
        {
            get
            {
                if(!_lastReportedCases.HasValue)
                {
                    var lastDay = Cases.Keys.Max();
                    var prevDay = lastDay.AddDays(-1);
                    int prev = Cases[prevDay];
                    int current = Cases[lastDay];
                    _lastReportedCases = current - prev;
                }
                return _lastReportedCases.Value;
            }
        }

        int? _lastReportedDeaths;

        public int LastReportedDeaths
        {
            get
            {
                if (!_lastReportedDeaths.HasValue)
                {
                    var lastDay = Deaths.Keys.Max();
                    var prevDay = lastDay.AddDays(-1);
                    int prev = Deaths[prevDay];
                    int current = Deaths[lastDay];
                    _lastReportedDeaths = current - prev;
                }
                return _lastReportedDeaths.Value;
            }
        }

        public int CasesWeeklyTrend =>
            (int)((double)DailyAverageCasesLastSevenDays / (double)DailyAverageCasesPreviousSevenDays * 100d - 100d);

        public int DeathsWeeklyTrend =>
            (int)((double)DailyAverageDeathsLastSevenDays / (double)DailyAverageDeathsPreviousSevenDays * 100d - 100d);
    }
}
