using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public record AddFileCommand(string Title, string Folder, string Vault) : IRequest;

    public class AddFileCommandHandler : IRequestHandler<AddFileCommand>
    {
        private readonly IProcessService _process;

        public AddFileCommandHandler(IProcessService process)
        {
            _process = process;
        }

        public async Task<Unit> Handle(AddFileCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<NotesConfiguration>("notes");
            if (!configuration.Configured)
                throw new UnconfiguredException("Please configure the Notes provider.");

            string vault = string.IsNullOrWhiteSpace(request.Vault) ? configuration.VaultName : request.Vault;
            string uri = $"obsidian://new?vault={vault}&name={request.Title}&append=true";

            _process.Open(uri);

            return await Unit.Task;
        }
    }
}
