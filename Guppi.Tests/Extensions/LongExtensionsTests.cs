using System;
using System.Collections.Generic;
using NUnit.Framework;

using Guppi.Application.Extensions;

namespace Guppi.Tests.Extensions
{
    public class LongExtensionsTests
    {
        [TestCaseSource(nameof(UnixTimestampData))]
        public void CanConvertUnixTimestampToDateTime(long timestamp, DateTime expected)
        {
            Assert.That(timestamp.UnixTimeStampToDateTime(), Is.EqualTo(expected));
        }

        public static IEnumerable<TestCaseData> UnixTimestampData => new []
        {
            new TestCaseData(1586633713, new DateTime(2020, 04, 11, 19, 35, 13, DateTimeKind.Utc).ToLocalTime()),
            new TestCaseData(1586631600, new DateTime(2020, 04, 11, 19, 00, 00, DateTimeKind.Utc).ToLocalTime()),
            new TestCaseData(1586725200, new DateTime(2020, 04, 12, 21, 00, 00, DateTimeKind.Utc).ToLocalTime()),
        };
    }
}
