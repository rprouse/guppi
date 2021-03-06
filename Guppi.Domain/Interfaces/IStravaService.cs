using System.Collections.Generic;
using System.Threading.Tasks;

namespace Guppi.Domain.Interfaces
{
    public interface IStravaService
    {
        Task<IEnumerable<Domain.Entities.Strava.StravaActivity>> GetActivities();

        void Configure();
    }
}
