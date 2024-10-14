using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guppi.Core.Services;

public interface IStravaService
{
    void Configure();
    Task<IEnumerable<Domain.Entities.Strava.Activity>> GetActivities();
}
