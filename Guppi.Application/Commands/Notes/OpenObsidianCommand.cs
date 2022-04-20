using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public record OpenObsidianCommand(string Vault, string Filename) : IRequest;

    public class OpenObsidianCommandHandler : IRequestHandler<OpenObsidianCommand>
    {
        private readonly IProcessService _process;

        public OpenObsidianCommandHandler(IProcessService process)
        {
            _process = process;
        }

        public async Task<Unit> Handle(OpenObsidianCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<NotesConfiguration>("notes");
            if (!configuration.Configured)
                throw new UnconfiguredException("Please configure the Notes provider.");

            string vault = string.IsNullOrWhiteSpace(request.Vault) ? configuration.VaultName : request.Vault;
            string uri = GetUriHandler(vault, request.Filename);

            _process.Open(uri);

            return await Unit.Task;
        }

        internal string GetUriHandler(string vault, string filename) =>
            string.IsNullOrWhiteSpace(filename) ?
                $"obsidian://open?vault={vault}" :
                $"obsidian://open?vault={vault}&file={filename}";
    }
}
