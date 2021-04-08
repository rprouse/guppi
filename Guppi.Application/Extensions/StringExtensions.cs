using System.Text.RegularExpressions;

namespace Guppi.Application.Extensions
{
    public static class StringExtensions
    {
        static readonly Regex emojiRegex = new Regex(@"(:[a-z_]+:)", RegexOptions.Compiled);

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
    }
}
