using FluentAssertions;
using Guppi.Application.Extensions;
using NUnit.Framework;

namespace Guppi.Tests.Extensions
{
    [TestFixture]
    public class StringExtensionTests
    {
        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase(" ", "")]
        [TestCase("A string", "A string")]
        [TestCase(":bell: You rang?", "You rang?")]
        [TestCase("You rang? :bell:", "You rang?")]
        [TestCase("Anomaly :fire:", "Anomaly")]
        [TestCase("Anomaly :fire: detected", "Anomaly detected")]
        [TestCase("No :white_exclamation_mark: not quite", "No not quite")]
        public void StripsEmoji(string str, string expected)
        {
            str.StripEmoji().Should().Be(expected);
        }
    }
}
