using System;
using NUnit.Framework;

using Guppi.Application.Extensions;

namespace Guppi.Tests.Extensions
{
    [TestFixture]
    public class FloatExtensionsTests
    {
        [TestCase(265.2f, -7)]
        [TestCase(281.07f, 7)]
        [TestCase(287.49f, 14)]
        public void CanConvertUnixTimestampToDateTime(float kalvin, int expected)
        {
            Assert.That(kalvin.KalvinToCelcius(), Is.EqualTo(expected));
        }
    }
}
