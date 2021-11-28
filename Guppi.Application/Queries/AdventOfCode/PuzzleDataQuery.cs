using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using Guppi.Domain.Entities.AdventOfCode;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.AdventOfCode
{

    public sealed record PuzzleDataQuery(int Year, int Day) : IRequest<string>;

    internal sealed class PuzzleDataQueryHandler : IRequestHandler<PuzzleDataQuery, string>
    {
        private readonly IHttpRestService _restService;

        public PuzzleDataQueryHandler(IHttpRestService restService)
        {
            _restService = restService;
        }

        public async Task<string> Handle(PuzzleDataQuery request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            if (!configuration.Configured)
            {
                throw new UnconfiguredException("Please configure the Advent of Code provider.");
            }

            _restService.AddHeader("Cookie", $"session={configuration.LoginToken}");
            return await _restService.GetStringAsync($"https://adventofcode.com/{request.Year}/day/{request.Day}/input");
        }
    }
}
