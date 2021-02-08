using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Hue;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Hue
{
    public sealed class ListBridgesQuery : IRequest<IEnumerable<HueBridge>>
    {
    }

    internal sealed class ListBridgesQueryHandler : IRequestHandler<ListBridgesQuery, IEnumerable<HueBridge>>
    {
        private readonly IHueService _hueService;

        public ListBridgesQueryHandler(IHueService hueService)
        {
            _hueService = hueService;
        }

        public async Task<IEnumerable<HueBridge>> Handle(ListBridgesQuery request, CancellationToken cancellationToken) =>
            await _hueService.ListBridges();
    }
}
