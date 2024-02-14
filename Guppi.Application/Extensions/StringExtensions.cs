using System.Text.RegularExpressions;

namespace Guppi.Application.Extensions
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

        [GeneratedRegex(@"(:[a-z_]+:)", RegexOptions.Compiled)]
        private static partial Regex EmojiRegex();
    }
}
