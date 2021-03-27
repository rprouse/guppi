using System.Net.Http;
using System.Net.Http.Headers;
using Guppi.Domain.Interfaces;
using Guppi.Infrastructure.Services;
using Guppi.Infrastructure.Services.Calendar;
using Guppi.Infrastructure.Services.Git;
using Guppi.Infrastructure.Services.Hue;
using Guppi.Infrastructure.Services.Notes;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
            services
                .AddSingleton<HttpClient>(serviceProvider => 
                {
                    var client = new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/guppi)");
                    return client;
                })
                .AddTransient<ICalendarService, CalendarService>()
                .AddTransient<IGitService, GitService>()
                .AddTransient<IHttpRestService, HttpRestService>()
                .AddTransient<IHueService, HueService>()
                .AddTransient<INotesService, NotesService>();
    }
}
