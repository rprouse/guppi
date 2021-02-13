using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{

    public sealed class ConfigureAocCommand : IRequest
    {
    }

    internal sealed class ConfigureAocCommandHandler : IRequestHandler<ConfigureAocCommand>
    {
        private readonly IAdventOfCodeService _aocService;

        public ConfigureAocCommandHandler(IAdventOfCodeService aocService)
        {
            _aocService = aocService;
        }

        public async Task<Unit> Handle(ConfigureAocCommand request, CancellationToken cancellationToken)
        {
            _aocService.Configure();
            return await Unit.Task;
        }
    }
}
