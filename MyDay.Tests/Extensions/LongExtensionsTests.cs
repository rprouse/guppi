using System;
using NUnit.Framework;

using MyDay.Core.Extensions;

namespace MyDay.Tests.Extensions
{
    public class LongExtensionsTests
    {
        [TestCase(1586633713, "2020-04-11 3:35:13 PM")]
        [TestCase(1586631600, "2020-04-11 3:00:00 PM")]
        [TestCase(1586725200, "2020-04-12 5:00:00 PM")]
        public void CanConvertUnixTimestampToDateTime(long timestamp, DateTime expected)
        {
            Assert.That(timestamp.UnixTimeStampToDateTime(), Is.EqualTo(expected));
        }
    }
}
