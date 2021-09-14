using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Guppi.Application.Exceptions;
using Guppi.Domain.Entities.Calendar;
using Guppi.Domain.Interfaces;
using Microsoft.Identity.Client;

namespace Guppi.Infrastructure.Services.Calendar
{
    internal sealed class Office365CalendarService : ICalendarService
    {
        const string Tenant = "common";
        const string Instance = "https://login.microsoftonline.com/";

        public async Task<IEnumerable<Event>> GetCalendarEvents(DateTime? minDate, DateTime? maxDate)
        {
            IPublicClientApplication publicClientApp = PublicClientApplicationBuilder.Create("b4b31f71-ccd0-4161-98d5-30871f959ec5")
                .WithRedirectUri("http://localhost:39428")
                .WithAuthority($"{Instance}{Tenant}")
                .Build();

            TokenCacheHelper.EnableSerialization(publicClientApp.UserTokenCache);

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
                        .WithPrompt(Prompt.SelectAccount)
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
                // TODO: Get Calendar Data
            }

            return new[] {
                new Event { Start = DateTime.Now.AddHours(1), End = DateTime.Now.AddHours(2), Summary = authResult?.AccessToken }
            };
        }

        public void Logout()
        {
            // Delete the token
        }
    }
}
