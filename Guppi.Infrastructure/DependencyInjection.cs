using Guppi.Domain.Interfaces;
using Guppi.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Guppi.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            return services.AddTransient<ICalendarService, CalendarService>();
        }
    }
}
