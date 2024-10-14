using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guppi.Core.Interfaces.Services;

public interface IStravaService
{
    void Configure();
    Task<IEnumerable<Entities.Strava.Activity>> GetActivities();
}
