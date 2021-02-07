using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public sealed class ConfigureNotesCommand : IRequest
    {
    }

    internal sealed class ConfigureNotesCommandHandler : IRequestHandler<ConfigureNotesCommand>
    {
        private readonly INotesService _notesService;

        public ConfigureNotesCommandHandler(INotesService notesService)
        {
            _notesService = notesService;
        }

        public async Task<Unit> Handle(ConfigureNotesCommand request, CancellationToken cancellationToken)
        {
            _notesService.Configure();
            return await Unit.Task;
        }
    }
}
