using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guppi.Application.Services;

public interface IStravaService
{
    void Configure();
    Task<IEnumerable<Domain.Entities.Strava.Activity>> GetActivities();
}
