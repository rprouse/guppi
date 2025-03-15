using System;

namespace Guppi.Core.Exceptions;

// Extension methods for Math since C# doesn't have Radians/Degrees conversion built-in
public static class MathExtensions
{
    public static double Radians(this double degrees) =>
        degrees * Math.PI / 180.0;

    public static double Degrees(this double radians) =>
        radians * 180.0 / Math.PI;
}
