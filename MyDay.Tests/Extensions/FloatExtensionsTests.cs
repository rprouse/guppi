using System;
using NUnit.Framework;

using MyDay.Core.Extensions;

namespace MyDay.Tests.Extensions
{
    [TestFixture]
    public class FloatExtensionsTests
    {
        [TestCase(265.2f, "-7°C")]
        [TestCase(281.07f, "7°C")]
        [TestCase(287.49f, "14°C")]
        public void CanConvertUnixTimestampToDateTime(float kalvin, string expected)
        {
            Assert.That(kalvin.KalvinToCelcius(), Is.EqualTo(expected));
        }
    }
}
