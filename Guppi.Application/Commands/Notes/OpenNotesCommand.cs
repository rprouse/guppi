using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.Notes
{
    public sealed class OpenNotesCommand : IRequest
    {
        public bool NoCreate { get; init; }
        public string Filename { get; init; }
    }

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
