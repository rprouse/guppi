using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Hue;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Hue
{
    public sealed class ListLightsQuery : IRequest<IEnumerable<HueLight>>
    {
        public string IpAddress { get; init; }

        public Action<string> WaitForUserInput { get; set; }
    }

    internal sealed class ListLightsQueryHandler : IRequestHandler<ListLightsQuery, IEnumerable<HueLight>>
    {
        private readonly IHueService _hueService;

        public ListLightsQueryHandler(IHueService hueService)
        {
            _hueService = hueService;
        }

        public async Task<IEnumerable<HueLight>> Handle(ListLightsQuery request, CancellationToken cancellationToken)
        {
            _hueService.WaitForUserInput = request.WaitForUserInput;
            return await _hueService.ListLights(request.IpAddress);
        }
    }
}
