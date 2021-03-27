using System;
using System.Linq;
using Guppi.Domain.Entities.Covid;

namespace Guppi.Application.Queries.Covid
{
#pragma warning disable IDE1006 // Naming Styles
    public class NyTimesCovidData
    {
        public string[] range { get; set; }
        public Datum[] data { get; set; }

        public CovidData GetCovidData() =>
            new CovidData 
            { 
                RegionData = data[0].GetRegionData(),
                SubRegionData = data.Skip(1).Select(d => d.GetRegionData()).ToList()
            };
    }

    public class Datum
    {
        public string display_name { get; set; }
        public string geoid { get; set; }
        public string link { get; set; }
        public string region_type { get; set; }
        public int population { get; set; }
        public Latest latest { get; set; }
        public string[] range { get; set; }
        public int[] cases { get; set; }
        public int[] deaths { get; set; }

        public RegionData GetRegionData()
        {
            var data = new RegionData
            {
                Name = display_name,
                GeoId = geoid,
                Link = link,
                RegionType = region_type,
                Population = population,
                LatestCases = latest.cases,
                LatestDeaths = latest.deaths,
                LatestDate = DateTime.Parse(range[1] ?? "1970-01-01")
            };
            var start = DateTime.Parse(range[0] ?? "1970-01-01");
            int days = (int)data.LatestDate.Subtract(start).TotalDays;

            for (int i = 0; i <= days; i++)
            {
                var current = start.AddDays(i);
                if (i < cases.Length)
                    data.Cases.Add(current, cases[i]);
                if (i < deaths.Length)
                    data.Deaths.Add(current, deaths[i]);
            }

            return data;
        }
    }

    public class Latest
    {
        public int cases { get; set; }
        public int deaths { get; set; }
    }
#pragma warning restore IDE1006 // Naming Styles
}
