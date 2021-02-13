using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{

    public sealed class RunTestsCommand : IRequest
    {
        public int Day { get; init; }
        public int Year { get; init; }
    }

    internal sealed class RunTestsCommandHandler : IRequestHandler<RunTestsCommand>
    {
        private readonly IAdventOfCodeService _aocService;

        public RunTestsCommandHandler(IAdventOfCodeService aocService)
        {
            _aocService = aocService;
        }

        public async Task<Unit> Handle(RunTestsCommand request, CancellationToken cancellationToken)
        {
            _aocService.RunTests(request.Year, request.Day);
            return await Unit.Task;
        }
    }
}
