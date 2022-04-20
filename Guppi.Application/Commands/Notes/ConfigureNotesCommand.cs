using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public sealed record ConfigureNotesCommand : IRequest;

    internal sealed class ConfigureNotesCommandHandler : IRequestHandler<ConfigureNotesCommand>
    {
        public async Task<Unit> Handle(ConfigureNotesCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<NotesConfiguration>("notes");
            configuration.RunConfiguration("Notes", "Set the Notes Directory.");
            return await Unit.Task;
        }
    }
}
