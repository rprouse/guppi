using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{

    public sealed class RunTestsCommand : IRequest
    {
        public int Day { get; init; }
        public int Year { get; init; }
    }

    internal sealed class RunTestsCommandHandler : IRequestHandler<RunTestsCommand>
    {
        public async Task<Unit> Handle(RunTestsCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");

            string dir = Path.Combine(configuration.SolutionDirectory, $"AdventOfCode{request.Year}");
            if (!Directory.Exists(dir))
            {
                throw new UnconfiguredException($"Project {dir} does not exist.");
            }

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "test" + GetTestFilter(request.Year, request.Day),
                WorkingDirectory = dir,
                UseShellExecute = false
            };
            var test = Process.Start(psi);
            test.WaitForExit();

            return await Unit.Task;
        }

        static string GetTestFilter(int year, int day) =>
            day == 0 ? "" : $" --filter FullyQualifiedName=AdventOfCode{year}.Day{day:00}Tests";
    }
}
