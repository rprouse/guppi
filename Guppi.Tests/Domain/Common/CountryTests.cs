using FluentAssertions;
using Guppi.Domain.Common;
using Guppi.Domain.Entities.Covid;
using NUnit.Framework;

namespace Guppi.Tests.Domain.Common
{
    [TestFixture]
    public class CountryTests
    {
        [TestCase("canada", Country.CAN)]
        [TestCase("Canada", Country.CAN)]
        [TestCase("CANADA", Country.CAN)]
        [TestCase("Brazil", Country.BRA)]
        [TestCase("Germany", Country.DEU)]
        [TestCase("Spain", Country.ESP)]
        [TestCase("France", Country.FRA)]
        [TestCase("UK", Country.GBR)]
        [TestCase("Great Britain", Country.GBR)]
        [TestCase("India", Country.IND)]
        [TestCase("Italy", Country.ITA)]
        [TestCase("mexico", Country.MEX)]
        [TestCase("United States", Country.USA)]
        [TestCase("USA", Country.USA)]
        [TestCase(null, Country.Unknown)]
        [TestCase("", Country.Unknown)]
        [TestCase("FantasyStan", Country.Unknown)]
        [TestCase("Turkey", Country.Unknown)]
        public void CanDetermineCountry(string country, Country expected)
        {
            country.GetCountry().Should().Be(expected);
        }
    }
}
