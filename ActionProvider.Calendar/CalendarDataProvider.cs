using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Spectre.Console;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Guppi.Core;

namespace ActionProvider.Calendar
{
    public class CalendarDataProvider : IActionProvider
    {
        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Guppi ActionProvider.Calendar";

        public Command GetCommand()
        {
            var view = new Command("view", "Views upcoming calendar events")
            {
                new Option<bool>(new string[]{"--agenda", "-a" }, "Displays today's agenda")
            };
            view.Handler = CommandHandler.Create(async (bool agenda) => await Execute(agenda));

            var logout = new Command("logout", "Logs out of the current Google account");
            logout.Handler = CommandHandler.Create(Logout);

            var cmd = new Command("calendar", "Display's today's calendar events")
            {
                view,
                logout
            };
            cmd.AddAlias("cal");
            return cmd;
        }

        private void Logout()
        {
            string token = Configuration.GetConfigurationFile("calendar_token");
            if (Directory.Exists(token))
                Directory.Delete(token, true);

            AnsiConsole.MarkupLine("[green][[:check_mark_button: Logged out of Google]][/]");
        }

        private async Task Execute(bool agenda)
        {
            string credentials = Configuration.GetConfigurationFile("calendar_credentials");
            if (!File.Exists(credentials))
            {
                AnsiConsole.MarkupLine("[yellow][[:yellow_circle: Please download the credentials. See the Readme.]][/]");
                return;
            }

            UserCredential credential = null;

            using (var stream = new FileStream(credentials, FileMode.Open, FileAccess.Read))
            {
                string token = Configuration.GetConfigurationFile("calendar_token");
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(token, true)).Result;
            }

            if (credential is null)
            {
                AnsiConsole.MarkupLine("[red][[:cross_mark: Failed to login to Google Calendar]][/]");
                return;
            }

            // Create Google Calendar API service.
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            var now = DateTime.Now;
            request.TimeMin = now;
            if(agenda) request.TimeMax = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            request.ShowHiddenInvitations = false;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = await request.ExecuteAsync();
            string title = agenda ? ":calendar: Today's agenda" : ":tear_off_calendar: Next event";

            AnsiConsoleHelper.TitleRule(title);

            try
            {
                if (events?.Items.Count > 0)
                {
                    bool found = false;
                    foreach (var eventItem in events.Items)
                    {
                        string start = eventItem.Start.DateTime?.ToString(agenda ? "HH:mm" : "MMM dd HH:mm");
                        if (string.IsNullOrEmpty(start))
                        {
                            continue;
                        }
                        string end = eventItem.End.DateTime?.ToString("-HH:mm") ?? "";
                        AnsiConsole.MarkupLine($"[white]{start}{end}\t[/][silver]{eventItem.Summary}[/]");
                        found = true;
                        if (!agenda) return;
                    }
                    if (found) return;
                }
                AnsiConsole.MarkupLine("[white][[No upcoming events found.]][/]");
            }
            finally
            {
                AnsiConsoleHelper.Rule("white");
            }
        }
    }
}
