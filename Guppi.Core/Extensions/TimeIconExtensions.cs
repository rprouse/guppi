using System;
using Spectre.Console;

namespace Guppi.Core.Extensions;

public static class TimeIconExtensions
{
    private static readonly string[] Clocks =
    [
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
    ];

    private static readonly string[] HalfClocks =
    [
        Emoji.Known.TwelveThirty,
        Emoji.Known.OneThirty,
        Emoji.Known.TwoThirty,
        Emoji.Known.ThreeThirty,
        Emoji.Known.FourThirty,
        Emoji.Known.FiveThirty,
        Emoji.Known.SixThirty,
        Emoji.Known.SevenThirty,
        Emoji.Known.EightThirty,
        Emoji.Known.NineThirty,
        Emoji.Known.TenThirty,
        Emoji.Known.ElevenThirty,
    ];

    /// <summary>
    /// Gets the clock emoji that matches the given time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetEmoji(this DateTime time)
    {
        int hour = time.Hour % 12;
        return time.Minute < 30 ? Clocks[hour] : HalfClocks[hour];
    }

    /// Gets the clock emoji that matches the given time
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    public static string GetEmoji(this DateTime? time) =>
        time is null ? Emoji.Known.TwelveOClock : time.Value.GetEmoji();
}
