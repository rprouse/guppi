using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Git
{
    public sealed class UnstageGitCommand : IRequest
    {
    }

    internal sealed class UnstageGitCommandHandler : IRequestHandler<UnstageGitCommand>
    {
        private readonly IGitService _gitService;

        public UnstageGitCommandHandler(IGitService gitService)
        {
            _gitService = gitService;
        }

        public async Task<Unit> Handle(UnstageGitCommand request, CancellationToken cancellationToken)
        {
            _gitService.RunGit("reset -q HEAD --");
            return await Unit.Task;
        }
    }
}
