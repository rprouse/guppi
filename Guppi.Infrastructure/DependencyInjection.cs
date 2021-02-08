using Guppi.Domain.Interfaces;
using Guppi.Infrastructure.Services.Calendar;
using Guppi.Infrastructure.Services.Hue;
using Guppi.Infrastructure.Services.Notes;
using Guppi.Infrastructure.Services.Weather;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services) =>
            services
                .AddTransient<ICalendarService, CalendarService>()
                .AddTransient<IHueService, HueService>()
                .AddTransient<INotesService, NotesService>()
                .AddTransient<IWeatherService, WeatherService>();
    }
}
