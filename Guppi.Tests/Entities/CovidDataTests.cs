using System;
using FluentAssertions;
using Guppi.Domain.Entities.Covid;
using NUnit.Framework;

namespace Guppi.Tests.Entities
{
    [TestFixture]
    public class CovidDataTests
    {
        RegionData _data;

        [SetUp]
        public void Setup()
        {
            var date = new DateTime(2021, 01, 31);
            _data = new RegionData
            {
                Name = "Test",
                Population = 1_000_000,
                LatestCases = 100_000,
                LatestDeaths = 1_000,
                LatestDate = date
            };

            for (int d = 0; d < 7; d++)
            {
                // Last seven days 1000 deaths per day and 10 deaths
                _data.Cases.Add(date.AddDays(-d), 100_000 - (d * 1000));
                _data.Deaths.Add(date.AddDays(-d), 1_000 - (d * 10));

                // Previous seven days, 2000 deaths per day and 20 deaths
                _data.Cases.Add(date.AddDays(-(d + 7)), 93_000 - (d * 2000));
                _data.Deaths.Add(date.AddDays(-(d + 7)), 930 - (d * 20));
            }
            _data.Cases.Add(date.AddDays(-14), 79_000);
            _data.Deaths.Add(date.AddDays(-14), 790);
        }

        [Test]
        public void CalculatesCasesPerHundredThousand()
        {
            _data.CasesPerHundredThousand.Should().Be(10000);
        }

        [Test]
        public void CalculatesDeathsPerHundredThousand()
        {
            _data.DeathsPerHundredThousand.Should().Be(100);
        }

        [Test]
        public void CalculatesDailyAverageCasesLastSevenDays()
        {
            _data.DailyAverageCasesLastSevenDays.Should().Be(1000);
        }

        [Test]
        public void CalculatesDailyAverageDeathsLastSevenDays()
        {
            _data.DailyAverageDeathsLastSevenDays.Should().Be(10);
        }

        [Test]
        public void CalculatesDailyAverageCasesLastSevenDaysPerHundredThousand()
        {
            _data.DailyAverageCasesLastSevenDaysPerHundredThousand.Should().Be(100);
        }

        [Test]
        public void CalculatesDailyAverageDeathsLastSevenDaysPerHundredThousand()
        {
            _data.DailyAverageDeathsLastSevenDaysPerHundredThousand.Should().Be(1);
        }

        [Test]
        public void CalculatesDailyAverageCasesPreviousSevenDays()
        {
            _data.DailyAverageCasesPreviousSevenDays.Should().Be(2000);
        }

        [Test]
        public void CalculatesDailyAverageDeathsPreviousSevenDays()
        {
            _data.DailyAverageDeathsPreviousSevenDays.Should().Be(20);
        }

        [Test]
        public void CalculatesLastReportedCases()
        {
            _data.LastReportedCases.Should().Be(1000);
        }

        [Test]
        public void CalculatesLastReportedDeaths()
        {
            _data.LastReportedDeaths.Should().Be(10);
        }

        [Test]
        public void CalculatesCasesWeeklyTrend()
        {
            _data.CasesWeeklyTrend.Should().Be(-50);
        }

        [Test]
        public void CalculatesDeathsWeeklyTrend()
        {
            _data.DeathsWeeklyTrend.Should().Be(-50);
        }
    }
}
