using System.Collections.Generic;
using Guppi.Application.Attributes;
using Guppi.Domain.Interfaces;
using Spectre.Console;

namespace Guppi.Application.Configurations
{
    public class CalendarConfiguration : Configuration
    {
        [Display("Enabled Calendars")]
        public List<EnabledCalendar> EnabledCalendars { get; set; }
            = new List<EnabledCalendar>();

        [Display("ICal Urls")]
        public List<string> ICalUrls { get; set; }
            = new List<string>();

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

        private void ConfigureEnabledCalendars()
        {
            var enabledCalendars = new List<EnabledCalendar>();
            foreach (var service in _calendarServices)
            {
                var yesno = AnsiConsole.Prompt<char>(
                    new TextPrompt<char>($"[green]Enable {service.Name} (y/N)?[/]")
                        .AddChoices(new[] { 'y', 'Y', 'n', 'N' })
                        .DefaultValue('n')
                        .ShowDefaultValue(false)
                        .ShowChoices(false)
                        .AllowEmpty()
                    );
                bool enabled = yesno == 'y' || yesno == 'Y';
                enabledCalendars.Add(new EnabledCalendar { Name = service.Name, Enabled = enabled });
            }
            EnabledCalendars = enabledCalendars;
        }

        private void ConfigureICalUrls()
        {
            // Delete configured URLS
            var delete = new List<string>();
            foreach (var url in ICalUrls)
            {
                var yesno = AnsiConsole.Prompt<char>(
                    new TextPrompt<char>($"[green]Delete {url} (y/N)?[/]")
                        .AddChoices(new[] { 'y', 'Y', 'n', 'N' })
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
