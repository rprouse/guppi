using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Event = Guppi.Domain.Entities.Calendar.Event;

namespace Guppi.Infrastructure.Services.Calendar
{
    internal sealed class Office365CalendarService : ICalendarService
    {
        const string ClientId = "b4b31f71-ccd0-4161-98d5-30871f959ec5";
        public string Name => "Office 365 Calendar";

        public async Task<IList<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
        {
            string accessToken = await Login();

            var graphClient = new GraphServiceClient(
                new DelegateAuthenticationProvider((requestMessage) =>
                {
                    requestMessage
                        .Headers
                        .Authorization = new AuthenticationHeaderValue("bearer", accessToken);

                    return Task.CompletedTask;
                }));

            var calendars = await graphClient.Me.Calendars
                .Request()
                .GetAsync();

            // TODO: Configure which calendar
            var calendar = calendars.FirstOrDefault(c => c.IsDefaultCalendar == true);

            var start = (minDate.HasValue ? minDate.Value : DateTime.Now).ToString("O");
            var end = (maxDate.HasValue ? maxDate.Value : DateTime.Now.AddDays(1)).ToString("O");
            var queryOptions = new List<QueryOption>()
            {
                new QueryOption("startDateTime", start),
                new QueryOption("endDateTime", end),
                new QueryOption("top", "100"),
            };
            var calendarView = await graphClient.Me.Calendars[calendar?.Id].CalendarView
                .Request(queryOptions)
                .GetAsync();

            var events = new List<Event>();
            foreach (var item in calendarView)
            {
                events.Add(new Event
                {
                    Start = DateTime.Parse(item.Start.DateTime).ToLocalTime(),
                    End = DateTime.Parse(item.End.DateTime).ToLocalTime(),
                    Summary = item.Subject
                }); ;
            }

            return events;
        }

        private static async Task<string> Login()
        {
            IPublicClientApplication publicClientApp = await CreatePublicClientApplication();

            var scopes = new[] { "calendars.read" };
            var accounts = await publicClientApp.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();
            AuthenticationResult result;
            try
            {
                result = await publicClientApp
                    .AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    result = await publicClientApp
                        .AcquireTokenInteractive(scopes)
                        .WithAccount(firstAccount)
                        .WithPrompt(Microsoft.Identity.Client.Prompt.SelectAccount)
                        .ExecuteAsync();
                }
                catch (MsalException msalex)
                {
                    throw new UnauthorizedException($"Failed to login to Office 365: {msalex.Message}");
                }
            }
            catch (Exception ex)
            {
                throw new UnauthorizedException($"Failed to login to Office 365: {ex.Message}");
            }

            var accessToken = result.AccessToken;
            return accessToken;
        }

        public async Task<string> Logout()
        {
            IPublicClientApplication publicClientApp = await CreatePublicClientApplication();
            var accounts = await publicClientApp.GetAccountsAsync();
            if (accounts.Any())
            {
                try
                {
                    await publicClientApp.RemoveAsync(accounts.FirstOrDefault());
                }
                catch (MsalException ex)
                {
                    return $"Failed to sign out of Office 365: {ex.Message}";
                }
            }
            return "Signed out of Office 365";
        }

        private static async Task<IPublicClientApplication> CreatePublicClientApplication()
        {
            var app = PublicClientApplicationBuilder
                .Create(ClientId)
                .WithRedirectUri("http://localhost")
                .Build();

            await TokenCacheHelper.RegisterCache(app.UserTokenCache);
            return app;
        }
    }
}
