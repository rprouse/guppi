using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.AdventOfCode;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.AdventOfCode
{

    public sealed class LeaderboardQuery : IRequest<Leaderboard>
    {
        public int Year { get; init; }
        public int Board { get; init; }
    }

    internal sealed class LeaderboardQueryHandler : IRequestHandler<LeaderboardQuery, Leaderboard>
    {
        private readonly IAdventOfCodeService _aocService;

        public LeaderboardQueryHandler(IAdventOfCodeService aocService)
        {
            _aocService = aocService;
        }

        public async Task<Leaderboard> Handle(LeaderboardQuery request, CancellationToken cancellationToken) =>
            await _aocService.GetLeaderboard(request.Year, request.Board);
    }
}
