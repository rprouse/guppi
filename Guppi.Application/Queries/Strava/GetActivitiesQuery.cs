using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Strava;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Strava
{
    public sealed class GetActivitiesQuery : IRequest<IEnumerable<StravaActivity>>
    {
    }

    internal sealed class GetActivitiesQueryHandler : IRequestHandler<GetActivitiesQuery, IEnumerable<StravaActivity>>
    {
        private readonly IStravaService _stravaService;

        public GetActivitiesQueryHandler(IStravaService stravaService)
        {
            _stravaService = stravaService;
        }

        public async Task<IEnumerable<StravaActivity>> Handle(GetActivitiesQuery request, CancellationToken cancellationToken)
        {
            return await _stravaService.GetActivities();
        }
    }
}
