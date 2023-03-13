using FluentAssertions;
using Guppi.Application.Services;
using NUnit.Framework;

namespace Guppi.Tests.Application.Queries.Ascii
{
    [TestFixture]
    public class AsciiQueryTests
    {
        AsciiService service;

        [SetUp]
        public void SetUp()
        {
            service = new AsciiService();
        }

        [Test]
        public void AsciiQueryHandler_Handle_Returns128Items()
        {
            var result = service.GetAsciiTable();
            result.Should().NotBeNull().And.HaveCount(128);
        }

        [Test]
        public void AsciiQueryHandler_Handle_ReturnsUniqueItems()
        {
            var result = service.GetAsciiTable();
            for (int i = 0; i < 128; i++)
            {
                result.Should().Contain(a => a.Value == i);
            }
        }
    }
}
