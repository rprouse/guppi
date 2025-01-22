using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using Guppi.Core;
using Guppi.Core.Configurations;
using Guppi.Core.Entities.Rss;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces.Services;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class NewsSkill(IRssService service) : ISkill
{
    private readonly IRssService _service = service;

    public IEnumerable<Command> GetCommands()
    {
        var markdown = new Option<bool>(["--markdown", "-m"], "Display as Markdown");

        var today = new Command("today", "Displays the latest news.") { markdown };
        today.AddAlias("latest");
        today.SetHandler(Today, markdown);

        var configure = new Command("configure", "Configure calendars");
        configure.AddAlias("config");
        configure.SetHandler(() => Configure());

        var cmd = new Command("news", "View latest news")
        {
            today,
            configure
        };
        cmd.AddAlias("rss");
        return [cmd];
    }

    private async Task Today(bool markdown)
    {
        try
        {
            var configuration = Configuration.Load<RssConfiguration>("Rss");
            if (!configuration.Enabled)
                throw new UnconfiguredException("Please configure RSS feeds");

            if (!markdown)
                AnsiConsoleHelper.TitleRule($":newspaper: Latest news");
            else
                AnsiConsole.MarkupLine($"## :newspaper: Latest news");

            AnsiConsole.WriteLine();
            foreach (var feed in configuration.Feeds)
            {
                try
                {
                    var newsFeed = await _service.ReadRssFeed(feed.Url);
                    var items = newsFeed.Items.OrderByDescending(i => i.Published).Take(feed.NumberOfItems);

                    if (markdown)
                    {
                        AnsiConsole.WriteLine($"### {newsFeed.Title}");
                        DisplayNewsAsMarkdown(items);
                        AnsiConsole.WriteLine();
                    }
                    else
                    {
                        AnsiConsole.MarkupLine($"[yellow]{newsFeed.Title}[/]");
                        DisplayNewsAsConsole(items);

                        AnsiConsole.WriteLine();

                        AnsiConsole.Prompt<string>(
                            new TextPrompt<string>("[green]ENTER to Continue...[/]")
                                .AllowEmpty()
                            );

                        AnsiConsole.Clear();
                    }
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine($"[red]:cross_mark: Error reading {feed.Name}:[/] {ex.Message}");
                }
            }
        }
        catch (UnconfiguredException ue)
        {
            AnsiConsole.MarkupLine($"[yellow][[:yellow_circle: ${ue.Message}]][/]");
        }
        finally
        {
            if (!markdown)
                AnsiConsoleHelper.Rule("white");
        }
    }

    private void DisplayNewsAsMarkdown(IEnumerable<NewsItem> items)
    {
        foreach (var item in items)
        {
            AnsiConsole.WriteLine();
            if (string.IsNullOrEmpty(item.Link))
                AnsiConsole.WriteLine($"#### {item.Title}");
            else
                AnsiConsole.WriteLine($"#### [{item.Title}]({item.Link})");

            AnsiConsole.WriteLine(item.Description);
            if (!string.IsNullOrWhiteSpace(item.Content))
            {
                AnsiConsole.WriteLine();
                AnsiConsole.WriteLine(item.Content);
            }
        }
    }

    private void DisplayNewsAsConsole(IEnumerable<NewsItem> items)
    {
        foreach (var item in items)
        {
            AnsiConsole.WriteLine();
            AnsiConsole.MarkupLine($"[white]{item.Title}[/]");
            AnsiConsole.WriteLine();

            // TODO: strip HTML
            AnsiConsole.WriteLine(item.Description);
            if (!string.IsNullOrEmpty(item.Link))
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[cyan]{item.Link}[/]");
            }
        }
    }

    private void Configure() => _service.Configure();
}
