using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Guppi.Application.Queries.Ascii;
using NUnit.Framework;

namespace Guppi.Tests.Application.Queries.Ascii
{
    [TestFixture]
    public class AsciiQueryTests
    {
        AsciiQueryHandler handler;

        [SetUp]
        public void SetUp()
        {
            handler = new AsciiQueryHandler();
        }

        [Test]
        public async Task AsciiQueryHandler_Handle_Returns128Items()
        {
            var result = await handler
                .Handle(new AsciiQuery(), new CancellationToken());

            result.Should().NotBeNull().And.HaveCount(128);
        }

        [Test]
        public async Task AsciiQueryHandler_Handle_ReturnsUniqueItems()
        {
            var result = await handler
                .Handle(new AsciiQuery(), new CancellationToken());

            for (int i = 0; i < 128; i++)
            {
                result.Should().Contain(a => a.Value == i);
            }
        }
    }
}
