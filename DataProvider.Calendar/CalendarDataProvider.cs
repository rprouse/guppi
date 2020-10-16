using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ColoredConsole;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Guppi.Core;

namespace DataProvider.Calendar
{
    public class CalendarDataProvider : IDataProvider
    {
        const string Command = "calendar";
        const string Name = "Calendar";

        static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Guppi DataProvider.Calendar";

        public Command GetCommand()
        {
            var view = new Command("view", "Views upcoming calendar events")
            {
                new Option<bool>(new string[]{"--all", "-a" }, "Displays today's agenda")
            };
            view.Handler = CommandHandler.Create(async (bool all) => await Execute(all));

            var logout = new Command("logout", "Logs out of the current Google account");
            logout.Handler = CommandHandler.Create(Logout);

            return new Command(Command, "Display's today's calendar events")
            {
                view,
                logout
            };
        }

        private void Logout()
        {
            string token = Configuration.GetConfigurationFile("calendar_token");
            if (Directory.Exists(token))
                Directory.Delete(token, true);

            ColorConsole.WriteLine("Logged out of Google".Yellow());
        }

        private async Task Execute(bool all)
        {
            string credentials = Configuration.GetConfigurationFile("calendar_credentials");
            if (!File.Exists(credentials))
            {
                ColorConsole.WriteLine("Please download the credentials. See the Readme.".Yellow());
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
                ColorConsole.WriteLine("Failed to login to Google Calendar".Red());
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
            if(all) request.TimeMax = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);
            request.ShowHiddenInvitations = false;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = await request.ExecuteAsync();
            if(all)
                ColorConsole.WriteLine("Today's agenda:".Yellow());
            else
                ColorConsole.WriteLine("Next event:".Yellow());
            Console.WriteLine();

            if (events?.Items.Count > 0)
            {
                bool found = false;
                foreach (var eventItem in events.Items)
                {
                    string when = eventItem.Start.DateTime?.ToString(all ? "t" : "g");
                    if (string.IsNullOrEmpty(when))
                    {
                        continue;
                    }
                    ColorConsole.Write($"{when}\t".Cyan());
                    ColorConsole.WriteLine(eventItem.Summary.White());
                    found = true;
                    if (!all) return;
                }
                if (found) return;
            }
            ColorConsole.WriteLine("No upcoming events found.".White());
        }
    }
}
