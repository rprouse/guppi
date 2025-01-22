using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Core.Attributes;
using Spectre.Console;

namespace Guppi.Core.Configurations;

public class RssConfiguration : Configuration
{
    [Display("RSS Feeds")]
    public List<RssFeed> Feeds { get; set; } = [];

    protected override void ConfigureCustomProperties()
    {
        ConfigureFeeds();
    }

    private static readonly char[] YES_NO = ['y', 'Y', 'n', 'N'];

    private void ConfigureFeeds()
    {
        var delete = new List<string>();
        foreach (var feed in Feeds)
        {
            var yesno = AnsiConsole.Prompt<char>(
                new TextPrompt<char>($"[green]Delete {feed.Name} (y/N)?[/]")
                    .AddChoices(YES_NO)
                    .DefaultValue('n')
                    .ShowDefaultValue(false)
                    .ShowChoices(false)
                    .AllowEmpty()
                );
            if (yesno == 'y' || yesno == 'Y')
            {
                delete.Add(feed.Name);
            }
        }
        foreach (var name in delete)
        {
            Feeds.RemoveAll(feed => feed.Name == name);
        }

        while (true)
        {
            var name = AnsiConsole.Prompt<string>(
                new TextPrompt<string>("[green]Feed Name <ENTER to Quit>:[/]")
                    .AllowEmpty()
                );
            if (string.IsNullOrWhiteSpace(name))
                break;

            var url = AnsiConsole.Prompt<string>(
            new TextPrompt<string>("[green]Feed URL:[/]")
                .AllowEmpty()
            );
            var numberOfItems = AnsiConsole.Prompt<int>(
            new TextPrompt<int>("[green]Number of items to display:[/]")
                .DefaultValue(5)
                .Validate(number => number > 0)
            );
            Feeds.Add(new RssFeed { Name = name, Url = url, NumberOfItems = numberOfItems });
        }
    }
}

public class RssFeed
{
    public string Name { get; set; }
    public string Url { get; set; }
    public int NumberOfItems { get; set; }
}
