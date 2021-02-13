using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Git
{

    public sealed class UpdateGitCommand : IRequest
    {
        public string Branch { get; init; }
    }

    internal sealed class UpdateGitCommandHandler : IRequestHandler<UpdateGitCommand>
    {
        private readonly IGitService _gitService;

        public UpdateGitCommandHandler(IGitService gitService)
        {
            _gitService = gitService;
        }

        public async Task<Unit> Handle(UpdateGitCommand request, CancellationToken cancellationToken)
        {
            _gitService.SwitchToBranch(request.Branch);
            _gitService.RunGit("fetch -p");
            _gitService.RunGit("pull");
            return await Unit.Task;
        }
    }
}
