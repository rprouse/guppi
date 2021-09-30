using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Guppi.Application.Exceptions;
using Guppi.Domain.Entities.Calendar;
using Guppi.Domain.Interfaces;
using Microsoft.Graph;
using Microsoft.Identity.Client;
using Event = Guppi.Domain.Entities.Calendar.Event;

namespace Guppi.Infrastructure.Services.Calendar
{
    internal sealed class Office365CalendarService : ICalendarService
    {
        const string ClientId = "b4b31f71-ccd0-4161-98d5-30871f959ec5";
        const string Tenant = "3b1e579a-142b-4e8d-b37e-b4851aae2439";
        const string Instance = "https://login.microsoftonline.com/";

        public async Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
        {
            IPublicClientApplication publicClientApp = CreatePublicClientApplication();

            AuthenticationResult authResult;
            var scopes = new[] { "user.read" };
            var accounts = await publicClientApp.GetAccountsAsync();
            var firstAccount = accounts.FirstOrDefault();
            try
            {
                authResult = await publicClientApp
                    .AcquireTokenSilent(scopes, firstAccount)
                    .ExecuteAsync();
            }
            catch (MsalUiRequiredException)
            {
                try
                {
                    authResult = await publicClientApp
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

            if (authResult != null)
            {
                try
                {
                    // Get Calendars
                    var authProvider = new GraphAuthenticationProvider(authResult);
                    var httpClient = GraphClientFactory.Create(authProvider);
                    var client = new GraphServiceClient(httpClient);
                    //var calendars = await client.Me.Calendars.Request().GetAsync();
                    var calendar = await client.Me.Calendar.Request().GetAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    throw;
                }
            }

            return new[] {
                new Event { Start = DateTime.Now.AddHours(1), End = DateTime.Now.AddHours(2), Summary = authResult?.AccessToken }
            };
        }

        public async Task<string> Logout()
        {
            IPublicClientApplication publicClientApp = CreatePublicClientApplication();
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

        private static IPublicClientApplication CreatePublicClientApplication()
        {
            IPublicClientApplication publicClientApp = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri("http://localhost:39428")
                .WithAuthority($"{Instance}{Tenant}")
                .Build();

            TokenCacheHelper.EnableSerialization(publicClientApp.UserTokenCache);
            return publicClientApp;
        }
    }

    internal class GraphAuthenticationProvider : IAuthenticationProvider
    {
        AuthenticationResult _token;

        public GraphAuthenticationProvider(AuthenticationResult token)
        {
            _token = token;
        }

        public Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            request.Headers.Add("Authorization", _token.CreateAuthorizationHeader());
            return Task.CompletedTask;
        }
    }
}
