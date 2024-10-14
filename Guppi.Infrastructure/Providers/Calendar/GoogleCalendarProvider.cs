using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Guppi.Core;
using Guppi.Core.Exceptions;
using Guppi.Core.Interfaces;

namespace Guppi.Core.Providers.Calendar;

internal sealed class GoogleCalendarProvider : ICalendarProvider
{
    static string[] Scopes = { CalendarService.Scope.CalendarReadonly };
    static string ApplicationName = "Guppi ActionProvider.Calendar";

    public string Name => "Google Calendar";

    public async Task<IList<Core.Entities.Calendar.Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
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
                GoogleClientSecrets.FromStream(stream).Secrets,
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
        var service = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });

        // Define parameters of request.
        EventsResource.ListRequest request = service.Events.List("primary");
        request.TimeMinDateTimeOffset = minDate;
        request.TimeMaxDateTimeOffset = maxDate;
        request.ShowHiddenInvitations = false;
        request.ShowDeleted = false;
        request.SingleEvents = true;
        request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

        // List events.
        Events events = await request.ExecuteAsync();

        return events.Items
            .Select(e => new Core.Entities.Calendar.Event 
            { 
                Start = e.Start.DateTimeDateTimeOffset?.LocalDateTime, 
                End = e.End.DateTimeDateTimeOffset?.LocalDateTime, 
                Summary = e.Summary,
                MeetingUrl = e.ConferenceData?.EntryPoints?.FirstOrDefault()?.Uri
            })
            .ToList();
    }

    public Task<string> Logout()
    {
        string token = Configuration.GetConfigurationFile("calendar_token");
        if (Directory.Exists(token))
        {
            Directory.Delete(token, true);
        }
        return Task.FromResult("Signed out of Google");
    }
}
