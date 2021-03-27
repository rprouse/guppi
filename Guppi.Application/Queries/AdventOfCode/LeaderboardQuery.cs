using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
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
        private readonly IHttpRestService _restService;

        public LeaderboardQueryHandler(IHttpRestService restService)
        {
            _restService = restService;
        }

        public async Task<Leaderboard> Handle(LeaderboardQuery request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            if (!configuration.Configured)
            {
                throw new UnconfiguredException("Please configure the Advent of Code provider.");
            }

            _restService.AddHeader("Cookie", $"session={configuration.LoginToken}");
            var leaders = await _restService.GetData<AocLeaderboard>($"https://adventofcode.com/{request.Year}/leaderboard/private/view/{request.Board}.json");
            return leaders.GetLeaderboard();
        }
    }
}
