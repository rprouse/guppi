namespace Guppi.Application.Extensions
{
    public static class FloatExtensions
    {
        public static int KalvinToCelcius(this float kalvin) =>
            (int)(kalvin - 273.15);
    }
}
