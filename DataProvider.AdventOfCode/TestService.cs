using System.Diagnostics;
using System.IO;
using ColoredConsole;

namespace DataProvider.AdventOfCode
{
    internal class TestService
    {
        AdventOfCodeConfiguration _configuration;

        public TestService(AdventOfCodeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void RunTests(int year, int day)
        {
            string dir = Path.Combine(_configuration.SolutionDirectory, $"AdventOfCode{year}");
            if (!Directory.Exists(dir))
            {
                ColorConsole.WriteLine($"[Project {dir} does not exist.]".Red());
                ColorConsole.WriteLine("[Configure the data provider to set the solution directory.]".Cyan());
                return;
            }

            var psi = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "test" + GetTestFilter(year, day),
                WorkingDirectory = dir,
                UseShellExecute = false
            };
            var test = Process.Start(psi);
            test.WaitForExit();
        }

        private string GetTestFilter(int year, int day) =>
            day == 0 ? "" : $" --filter FullyQualifiedName=AdventOfCode{year}.Day{day:00}Tests";
    }
}
