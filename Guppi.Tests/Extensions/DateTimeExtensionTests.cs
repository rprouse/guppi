using System;
using Guppi.Core.Extensions;
using NUnit.Framework;
using Spectre.Console;

namespace Guppi.Tests.Extensions
{
    [TestFixture]
    public class DateTimeExtensionTests
    {
        [TestCase("2021-01-01 00:12:00", Emoji.Known.TwelveOClock)]
        [TestCase("2021-01-01 01:50:00", Emoji.Known.OneOClock)]
        [TestCase("2021-01-01 02:46:00", Emoji.Known.TwoOClock)]
        [TestCase("2021-01-01 03:17:00", Emoji.Known.ThreeOClock)]
        [TestCase("2021-01-01 04:11:00", Emoji.Known.FourOClock)]
        [TestCase("2021-01-01 05:54:00", Emoji.Known.FiveOClock)]
        [TestCase("2021-01-01 06:33:00", Emoji.Known.SixOClock)]
        [TestCase("2021-01-01 07:22:00", Emoji.Known.SevenOClock)]
        [TestCase("2021-01-01 08:19:59", Emoji.Known.EightOClock)]
        [TestCase("2021-01-01 09:00:00", Emoji.Known.NineOClock)]
        [TestCase("2021-01-01 10:32:00", Emoji.Known.TenOClock)]
        [TestCase("2021-01-01 11:59:59", Emoji.Known.ElevenOClock)]
        [TestCase("2021-01-01 12:12:00", Emoji.Known.TwelveOClock)]
        [TestCase("2021-01-01 13:50:00", Emoji.Known.OneOClock)]
        [TestCase("2021-01-01 14:46:00", Emoji.Known.TwoOClock)]
        [TestCase("2021-01-01 15:17:00", Emoji.Known.ThreeOClock)]
        [TestCase("2021-01-01 16:11:00", Emoji.Known.FourOClock)]
        [TestCase("2021-01-01 17:54:00", Emoji.Known.FiveOClock)]
        [TestCase("2021-01-01 18:33:00", Emoji.Known.SixOClock)]
        [TestCase("2021-01-01 19:22:00", Emoji.Known.SevenOClock)]
        [TestCase("2021-01-01 20:19:59", Emoji.Known.EightOClock)]
        [TestCase("2021-01-01 21:00:00", Emoji.Known.NineOClock)]
        [TestCase("2021-01-01 22:32:00", Emoji.Known.TenOClock)]
        [TestCase("2021-01-01 23:59:59", Emoji.Known.ElevenOClock)]
        public void CanConvertTimeToEmoji(DateTime time, string emoji)
        {
            Assert.That(time.GetEmoji(), Is.EqualTo(emoji));
        }

        [TestCase("2021-01-01 00:12:00", Emoji.Known.TwelveOClock)]
        [TestCase("2021-01-01 01:50:00", Emoji.Known.OneOClock)]
        [TestCase("2021-01-01 02:46:00", Emoji.Known.TwoOClock)]
        [TestCase("2021-01-01 03:17:00", Emoji.Known.ThreeOClock)]
        [TestCase("2021-01-01 04:11:00", Emoji.Known.FourOClock)]
        [TestCase("2021-01-01 05:54:00", Emoji.Known.FiveOClock)]
        [TestCase("2021-01-01 06:33:00", Emoji.Known.SixOClock)]
        [TestCase("2021-01-01 07:22:00", Emoji.Known.SevenOClock)]
        [TestCase("2021-01-01 08:19:59", Emoji.Known.EightOClock)]
        [TestCase("2021-01-01 09:00:00", Emoji.Known.NineOClock)]
        [TestCase("2021-01-01 10:32:00", Emoji.Known.TenOClock)]
        [TestCase("2021-01-01 11:59:59", Emoji.Known.ElevenOClock)]
        [TestCase("2021-01-01 12:12:00", Emoji.Known.TwelveOClock)]
        [TestCase("2021-01-01 13:50:00", Emoji.Known.OneOClock)]
        [TestCase("2021-01-01 14:46:00", Emoji.Known.TwoOClock)]
        [TestCase("2021-01-01 15:17:00", Emoji.Known.ThreeOClock)]
        [TestCase("2021-01-01 16:11:00", Emoji.Known.FourOClock)]
        [TestCase("2021-01-01 17:54:00", Emoji.Known.FiveOClock)]
        [TestCase("2021-01-01 18:33:00", Emoji.Known.SixOClock)]
        [TestCase("2021-01-01 19:22:00", Emoji.Known.SevenOClock)]
        [TestCase("2021-01-01 20:19:59", Emoji.Known.EightOClock)]
        [TestCase("2021-01-01 21:00:00", Emoji.Known.NineOClock)]
        [TestCase("2021-01-01 22:32:00", Emoji.Known.TenOClock)]
        [TestCase("2021-01-01 23:59:59", Emoji.Known.ElevenOClock)]
        [TestCase(null, Emoji.Known.TwelveOClock)]
        public void CanConvertNullableTimeToEmoji(DateTime? time, string emoji)
        {
            Assert.That(time.GetEmoji(), Is.EqualTo(emoji));
        }
    }
}
