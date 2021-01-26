using System;
using Spectre.Console;

namespace Guppi.Core.Extensions
{
    public static class TimeIconExtensions
    {
        private static string[] Clocks = new[]
        {
            Emoji.Known.TwelveOClock,
            Emoji.Known.OneOClock,
            Emoji.Known.TwoOClock,
            Emoji.Known.ThreeOClock,
            Emoji.Known.FourOClock,
            Emoji.Known.FiveOClock,
            Emoji.Known.SixOClock,
            Emoji.Known.SevenOClock,
            Emoji.Known.EightOClock,
            Emoji.Known.NineOClock,
            Emoji.Known.TenOClock,
            Emoji.Known.ElevenOClock,
        };

        /// <summary>
        /// Gets the clock emoji that matches the given time
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string GetEmoji(this DateTime time)
        {
            int hour = time.Hour % 12;
            return Clocks[hour];
        }
    }
}
