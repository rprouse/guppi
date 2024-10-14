using System;

namespace Guppi.Core.Extensions;

public static  class TemperatureExtensions
{
    public static string ToColor(this int celsius) =>
        celsius switch
        {
            < 0 => "#0099FF",               // Very Cold: Light Blue
            >= 0 and < 5 => "#00CCFF",      // Cold: Sky Blue
            >= 5 and < 15 => "#00FFFF",     // Cool: Cyan
            >= 15 and < 20 => "#00FF99",    // Mild: Mint Green
            >= 20 and < 25 => "#99FF00",    // Warm: Lime Green
            >= 25 and < 30 => "#FFCC00",    // Hot: Amber
            >= 30 and < 35 => "#FF6600",    // Very Hot: Orange
            >= 35 => "#FF0000",             // Extreme Heat: Red
        };

    public static string ToColor(this float celsius) =>
        ((int)Math.Round(celsius)).ToColor();
}
