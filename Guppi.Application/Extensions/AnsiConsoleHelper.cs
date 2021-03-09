using Spectre.Console;

namespace Guppi.Application.Extensions
{
    public static class AnsiConsoleHelper
    {
        public static void Rule(string style)
        {
            var rule = new Rule();
            rule.RuleStyle(style);
            AnsiConsole.Render(rule);
        }

        public static void TitleRule(string title, string color = "white")
        {
            var rule = new Rule($"[{color}][[{title}]][/]");
            rule.Alignment = Justify.Left;
            rule.RuleStyle(color);
            AnsiConsole.Render(rule);
            AnsiConsole.WriteLine();
        }
    }
}
