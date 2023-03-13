using System.Reflection;
using Guppi.Application.Services;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services
            .AddMediatR(Assembly.GetExecutingAssembly())
            .AddTransient<IAdventOfCodeService, AdventOfCodeService>()
            .AddTransient<IAsciiService, AsciiService>()
            .AddTransient<ICalendarService, CalendarService>()
            .AddTransient<IGitService, GitService>();

        return services;
    }
}
