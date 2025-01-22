using Shouldly;
using Guppi.Core.Configurations;
using NUnit.Framework;

namespace Guppi.Tests.Application.Configuration
{
    [TestFixture]
    public class AdventOfCodeConfigurationTests
    {
        [TestCase("", "")]
        [TestCase("abcd", "abcd")]
        [TestCase("session=abcd", "abcd")]
        [TestCase("Session=abcd", "abcd")]
        [TestCase("SESSION=abcd", "abcd")]
        public void LoginTokenStripsSession(string value, string expected)
        {
            var config = new AdventOfCodeConfiguration();
            config.LoginToken = value;
            config.LoginToken.ShouldBe(expected);
        }
    }
}
