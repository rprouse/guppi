using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guppi.Core.Interfaces;

public interface IStravaService
{
    void Configure();
    Task<IEnumerable<Entities.Strava.Activity>> GetActivities();
}
