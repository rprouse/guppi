using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using Guppi.Core.Attributes;
using Guppi.Domain.Interfaces;
using Spectre.Console;

namespace Guppi.Core.Configurations
{
    public class CalendarConfiguration : Configuration
    {
        [Display("Enabled Calendars")]
        public List<EnabledCalendar> EnabledCalendars { get; set; } = [];

        [Display("ICal Urls")]
        public List<string> ICalUrls { get; set; } = [];

        IEnumerable<ICalendarService> _calendarServices;
        public void SetCalendarServices(IEnumerable<ICalendarService> calendarServices)
        {
            _calendarServices = calendarServices;
        }

        protected override void ConfigureCustomProperties()
        {
            ConfigureEnabledCalendars();
            ConfigureICalUrls();
        }

        private static readonly char[] YES_NO = [ 'y', 'Y', 'n', 'N' ];

        private void ConfigureEnabledCalendars()
        {
            EnabledCalendars = _calendarServices.Select(service => 
            {
                var yesno = AnsiConsole.Prompt<char>(
                    new TextPrompt<char>($"[green]Enable {service.Name} (y/N)?[/]")
                        .AddChoices(YES_NO)
                        .DefaultValue('n')
                        .ShowDefaultValue(false)
                        .ShowChoices(false)
                        .AllowEmpty()
                    );
                bool enabled = yesno == 'y' || yesno == 'Y';
                return new EnabledCalendar { Name = service.Name, Enabled = enabled };
            }).ToList();
        }

        private void ConfigureICalUrls()
        {
            // Delete configured URLS
            var delete = new List<string>();
            foreach (var url in ICalUrls)
            {
                var yesno = AnsiConsole.Prompt<char>(
                    new TextPrompt<char>($"[green]Delete {url} (y/N)?[/]")
                        .AddChoices(YES_NO)
                        .DefaultValue('n')
                        .ShowDefaultValue(false)
                        .ShowChoices(false)
                        .AllowEmpty()
                    );
                if (yesno == 'y' || yesno == 'Y') delete.Add(url);
            }
            foreach (var url in delete)
                ICalUrls.Remove(url);

            while (true)
            {
                AnsiConsole.WriteLine();
                string url = AnsiConsole.Prompt<string>(
                    new TextPrompt<string>($"[green]Add iCal URL <ENTER to Quit>:[/] ")
                        .AllowEmpty()
                );
                if (string.IsNullOrWhiteSpace(url)) break;
                ICalUrls.Add(url.Trim());
            }
        }
    }

    public class EnabledCalendar
    {
        public string Name { get; set; }
        public bool Enabled { get; set; }
    }
}
