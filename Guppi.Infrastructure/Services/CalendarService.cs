using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Domain.Entities;
using Guppi.Domain.Interfaces;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

using GoogleCalendarService = Google.Apis.Calendar.v3.CalendarService;
using System.IO;
using System.Threading;
using Guppi.Application.Exceptions;
using Guppi.Application;

namespace Guppi.Infrastructure.Services
{
    internal class CalendarService : ICalendarService
    {
        static string[] Scopes = { GoogleCalendarService.Scope.CalendarReadonly };
        static string ApplicationName = "Guppi ActionProvider.Calendar";

        public async Task<IEnumerable<CalendarEvent>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
        {
            string credentials = Configuration.GetConfigurationFile("calendar_credentials");
            if (!File.Exists(credentials))
            {
                throw new UnconfiguredException("Please download the credentials. See the Readme.");
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
                throw new UnauthorizedException("Failed to login to Google Calendar");
            }

            // Create Google Calendar API service.
            var service = new GoogleCalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            EventsResource.ListRequest request = service.Events.List("primary");
            var now = DateTime.Now;
            request.TimeMin = minDate;
            request.TimeMax = maxDate;
            request.ShowHiddenInvitations = false;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            // List events.
            Events events = await request.ExecuteAsync();

            return events.Items.Select(e => new CalendarEvent { Start = e.Start.DateTime, End = e.End.DateTime, Summary = e.Summary });
        }

        public void Logout()
        {
            string token = Configuration.GetConfigurationFile("calendar_token");
            if (Directory.Exists(token))
                Directory.Delete(token, true);
        }
    }
}
