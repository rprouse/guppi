using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Git
{
    public sealed class AmmendGitCommand : IRequest
    {
    }

    internal sealed class AmmendGitCommandHandler : IRequestHandler<AmmendGitCommand>
    {
        private readonly IGitService _gitService;

        public AmmendGitCommandHandler(IGitService gitService)
        {
            _gitService = gitService;
        }

        public async Task<Unit> Handle(AmmendGitCommand request, CancellationToken cancellationToken)
        {
            _gitService.RunGit("commit --amend --no-edit");
            return await Unit.Task;
        }
    }
}
