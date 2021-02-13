using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{

    public sealed class AddDayCommand : IRequest<AddDayDto>
    {
        public int Year { get; init; }
    }

    internal sealed class AddDayCommandHandler : IRequestHandler<AddDayCommand, AddDayDto>
    {
        private readonly IAdventOfCodeService _aocService;

        public AddDayCommandHandler(IAdventOfCodeService aocService)
        {
            _aocService = aocService;
        }

        public async Task<AddDayDto> Handle(AddDayCommand request, CancellationToken cancellationToken)
        {
            (string dir, int newDay) = _aocService.AddDayTo(request.Year);
            return await Task.FromResult(new AddDayDto { Directory = dir, NewDay = newDay });
        }
    }
}
