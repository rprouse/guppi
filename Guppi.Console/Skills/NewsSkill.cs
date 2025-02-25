using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Core;
using Guppi.Core.Configurations;
using Guppi.Core.Entities.Rss;
using Guppi.Core.Exceptions;
using Guppi.Core.Extensions;
using Guppi.Core.Interfaces.Services;
using Spectre.Console;

namespace Guppi.Console.Skills;

internal class NewsSkill(IRssService service, CalendarSkill calendar) : ISkill
{
    private readonly IRssService _service = service;
    private readonly CalendarSkill _calendar = calendar;

    public IEnumerable<Command> GetCommands()
    {
        var markdown = new Option<bool>(["--markdown", "-m"], "Display as Markdown");

        var today = new Command("today", "Displays the latest news.") { markdown };
        today.AddAlias("latest");
        today.SetHandler(Today, markdown);

        var remarkable = new Command("remarkable", "Creates a PDF newspaper that is uploaded to a Remarkable tablet.");
        remarkable.SetHandler(Remarkable);

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

    private async Task Remarkable(InvocationContext context)
    {
        StringBuilder sb = new();

        var now = DateTime.Now;
        var midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Local);

        sb.AppendLine($"# {now.ToLongDateString()}");
        sb.AppendLine($"## Today's agenda");
        sb.AppendLine(await _calendar.Agenda(midnight, "Today's agenda", true, true));

        sb.AppendLine(await Today(true));
    }

    private async Task<string> Today(bool markdown)
    {
        StringBuilder sb = new();
        try
        {

            var configuration = Configuration.Load<RssConfiguration>("Rss");
            if (!configuration.Enabled)
                throw new UnconfiguredException("Please configure RSS feeds");

            if (!markdown)
            {
                AnsiConsoleHelper.TitleRule($":newspaper: Latest news");
                AnsiConsole.WriteLine();
            }
            else
                sb.AppendLine($"## 📰 Latest news");

            foreach (var feed in configuration.Feeds)
            {
                try
                {
                    var newsFeed = await _service.ReadRssFeed(feed.Url);
                    var items = newsFeed.Items.OrderByDescending(i => i.Published).Take(feed.NumberOfItems);

                    if (markdown)
                    {
                        sb.AppendLine($"### {newsFeed.Title}");
                        sb.AppendLine(DisplayNewsAsMarkdown(items));
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
            System.Console.Write(sb.ToString());
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
        return sb.ToString();
    }

    private string DisplayNewsAsMarkdown(IEnumerable<NewsItem> items)
    {
        StringBuilder sb = new ();
        foreach (var item in items)
        {
            sb.AppendLine();
            if (string.IsNullOrEmpty(item.Link))
                sb.AppendLine($"#### {item.Title}");
            else
                sb.AppendLine($"#### [{item.Title}]({item.Link})");

            sb.AppendLine(item.Description);
            if (!string.IsNullOrWhiteSpace(item.Content))
            {
                sb.AppendLine();
                sb.AppendLine(item.Content);
            }
        }
        return sb.ToString();
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
