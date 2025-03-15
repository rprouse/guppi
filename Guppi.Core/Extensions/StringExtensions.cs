using System.Text.RegularExpressions;

namespace Guppi.Core.Extensions
{
    public static partial class StringExtensions
    {
        static readonly Regex emojiRegex = EmojiRegex();

        public static string StripEmoji(this string str)
        {
            if (str is not null)
            { 
                str = emojiRegex
                    .Replace(str, "")
                    .Replace("  ", " ")
                    .Trim();
            }
            return str;
        }

        public static double ToDouble(this string str) =>
            double.TryParse(str, out double result) ? result : 0;

        [GeneratedRegex(@"(:[a-z_]+:)", RegexOptions.Compiled)]
        private static partial Regex EmojiRegex();
    }
}
