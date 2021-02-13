using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Git
{
    public sealed class UndoGitCommand : IRequest
    {
        public bool Hard { get; set; }
    }

    internal sealed class UndoGitCommandHandler : IRequestHandler<UndoGitCommand>
    {
        private readonly IGitService _gitService;

        public UndoGitCommandHandler(IGitService gitService)
        {
            _gitService = gitService;
        }

        public async Task<Unit> Handle(UndoGitCommand request, CancellationToken cancellationToken)
        {
            _gitService.RunGit(string.Format("reset --{0} HEAD~1", request.Hard ? "hard" : "soft"));
            return await Unit.Task;
        }
    }
}
