namespace Guppi.Application.Extensions
{
    public static class FloatExtensions
    {
        public static string KalvinToCelcius(this float kalvin)
        {
            int c = (int)(kalvin - 273.15);
            return $"{c}°C";
        }
    }
}
