using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public record OpenNotesCommand(string Filename, bool NoCreate) : IRequest;

    internal sealed class OpenNotesCommandHandler : IRequestHandler<OpenNotesCommand>
    {
        private readonly INotesService _notesService;

        public OpenNotesCommandHandler(INotesService notesService)
        {
            _notesService = notesService;
        }

        public async Task<Unit> Handle(OpenNotesCommand request, CancellationToken cancellationToken)
        {
            _notesService.OpenNotes(request.NoCreate, request.Filename);
            return await Unit.Task;
        }
    }
}
