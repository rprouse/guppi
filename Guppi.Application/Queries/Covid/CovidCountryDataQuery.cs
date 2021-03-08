using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Covid;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Covid
{
    public sealed class CovidCountryDataQuery : IRequest<CovidData>
    {
        public Country Country { get; init; }
    }

    internal sealed class CovidCountryDataQueryHandler : IRequestHandler<CovidCountryDataQuery, CovidData>
    {
        private readonly ICovidService _service;

        public CovidCountryDataQueryHandler(ICovidService service)
        {
            _service = service;
        }

        public async Task<CovidData> Handle(CovidCountryDataQuery request, CancellationToken cancellationToken) =>
            await _service.GetCovidData(request.Country);
    }
}
