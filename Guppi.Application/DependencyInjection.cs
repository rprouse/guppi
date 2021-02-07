using System.Reflection;
using ActionProvider.AdventOfCode;
using ActionProvider.Calendar;
using ActionProvider.Git;
using ActionProvider.Hue;
using ActionProvider.Notes;
using ActionProvider.Utilities;
using ActionProvider.Weather;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly())
                .AddTransient<IActionProvider, AdventOfCodeDataProvider>()
                .AddTransient<IActionProvider, CalendarDataProvider>()
                .AddTransient<IActionProvider, GitDataProvider>()
                .AddTransient<IActionProvider, HueLightsDataProvider>()
                .AddTransient<IActionProvider, NotesProvider>()
                .AddTransient<IActionProvider, WeatherDataProvider>()
                .AddTransient<IMultipleActionProvider, UtilitiesProvider>();

            return services;
        }
    }
}
