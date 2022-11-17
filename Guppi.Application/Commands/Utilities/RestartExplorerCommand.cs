using System;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Utilities
{
    public record RestartExplorerCommand : IRequest;

    public class RestartExplorerCommandHandler : IRequestHandler<RestartExplorerCommand>
    {
        const string Explorer = "explorer";

        readonly IProcessService _process;

        public RestartExplorerCommandHandler(IProcessService process)
        {
            _process = process;
        }

        public async Task<Unit> Handle(RestartExplorerCommand request, CancellationToken cancellationToken)
        {
            _process.Kill(Explorer);
            // Wait to give Explorer a chance to restart itself
            await Task.Delay(500);
            if (!_process.Running(Explorer))
                _process.Start(Explorer);

            return await Unit.Task;
        }
    }
}
