using Spectre.Console;

namespace Guppi.Application.Extensions
{
    public static class AnsiConsoleHelper
    {
        public static void Rule(string style)
        {
            var rule = new Rule();
            rule.RuleStyle(style);
            AnsiConsole.Write(rule);
        }

        public static void TitleRule(string title, string color = "white")
        {
            var rule = new Rule($"[{color}][[{title}]][/]");
            rule.Alignment = Justify.Left;
            rule.RuleStyle(color);
            AnsiConsole.Write(rule);
            AnsiConsole.WriteLine();
        }

        public static void PressEnterToContinue()
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine("[red][[Press ENTER to continue...]][/]");
            System.Console.ReadLine();
        }
    }
}
