using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{

    public sealed class ConfigureAocCommand : IRequest
    {
    }

    internal sealed class ConfigureAocCommandHandler : IRequestHandler<ConfigureAocCommand>
    {
        public async Task<Unit> Handle(ConfigureAocCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            configuration.RunConfiguration("Advent of Code", "Enter the Advent of Code session cookie value.");
            return await Unit.Task;
        }
    }
}
