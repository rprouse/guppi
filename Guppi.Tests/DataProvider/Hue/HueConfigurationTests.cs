using Guppi.Infrastructure.Services.Hue;
using NUnit.Framework;

namespace Guppi.Tests.ActionProvider.Hue
{
    public class HueConfigurationTests
    {
        [TestCase(null, 0U)]
        [TestCase("", 0U)]
        [TestCase("   ", 0U)]
        [TestCase("Garbage", 0U)]
        [TestCase("-1", 0U)]
        [TestCase("1", 1U)]
        [TestCase("12", 12U)]
        [TestCase("12.0", 0U)]
        [TestCase("12.5", 0U)]
        public void ConvertsDefaultLight(string str, uint expected)
        {
            var config = new HueConfiguration { DefaultLight = str };
            Assert.That(config.GetDefaultLight(), Is.EqualTo(expected));
        }
    }
}
