using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Hue
{
    public sealed class GetDefaultLightQuery : IRequest<uint>
    {
    }

    internal sealed class GetDefaultLightQueryHandler : IRequestHandler<GetDefaultLightQuery, uint>
    {
        private readonly IHueService _hueService;

        public GetDefaultLightQueryHandler(IHueService hueService)
        {
            _hueService = hueService;
        }

        public async Task<uint> Handle(GetDefaultLightQuery request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<HueConfiguration>("hue");
            return await Task.FromResult(configuration.GetDefaultLight());
        }
    }
}
