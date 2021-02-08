using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Hue
{
    public sealed class ConfigureHueCommand : IRequest
    {
    }

    internal sealed class ConfigureHueCommandHandler : IRequestHandler<ConfigureHueCommand>
    {
        private readonly IHueService _hueService;

        public ConfigureHueCommandHandler(IHueService hueService)
        {
            _hueService = hueService;
        }

        public async Task<Unit> Handle(ConfigureHueCommand request, CancellationToken cancellationToken)
        {
            _hueService.Configure();
            return await Unit.Task;
        }
    }
}
