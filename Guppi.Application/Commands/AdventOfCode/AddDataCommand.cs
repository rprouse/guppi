using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{
    public sealed record AddDataCommand(int Year, int Day, string Data) : IRequest;

    internal sealed class AddDataCommandHandler : IRequestHandler<AddDataCommand>
    {
        public async Task<Unit> Handle(AddDataCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            string file = Path.Combine(configuration.SolutionDirectory, $"AdventOfCode{request.Year}", $"Day{request.Day:00}", "Data.txt");
            if (!File.Exists(file))
            {
                throw new WarningException($"Data file {file} does not exist.");
            }
            await File.WriteAllTextAsync(file, request.Data);
            return await Unit.Task;
        }
    }
}
