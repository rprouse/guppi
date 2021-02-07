using System;
using System.Collections.Generic;
using System.Text;
using Spectre.Console;

namespace Guppi.Application
{
    public static class AnsiConsoleHelper
    {
        public static void Rule(string style)
        {
            var rule = new Rule();
            rule.RuleStyle(style);
            AnsiConsole.Render(rule);
        }

        public static void TitleRule(string title)
        {
            var rule = new Rule($"[white][[{title}]][/]");
            rule.Alignment = Justify.Left;
            rule.RuleStyle("white");
            AnsiConsole.Render(rule);
            AnsiConsole.WriteLine();
        }
    }
}
