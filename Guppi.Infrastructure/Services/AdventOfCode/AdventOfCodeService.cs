using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Guppi.Application;
using Guppi.Application.Exceptions;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.AdventOfCode
{
    public class AdventOfCodeService : IAdventOfCodeService
    {
        AdventOfCodeConfiguration _configuration;
        private readonly IHttpRestService _restService;

        public AdventOfCodeService(IHttpRestService restService)
        {
            _configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            _restService = restService;
        }

        public void Configure()
        {
            _configuration.RunConfiguration("Advent of Code", "Enter the Advent of Code session cookie value.");
        }

        public async Task<Domain.Entities.AdventOfCode.Leaderboard> GetLeaderboard(int year, int board)
        {
            if (!_configuration.Configured)
            {
                throw new UnconfiguredException("Please configure the Advent of Code provider.");
            }

            _restService.AddHeader("Cookie", $"session={_configuration.LoginToken}");
            var leaders = await _restService.GetData<Leaderboard>($"https://adventofcode.com/{year}/leaderboard/private/view/{board}.json");
            return leaders.GetLeaderboard();
        }

        public (string directory, int newDay) AddDayTo(int year)
        {
            string dir = Path.Combine(_configuration.SolutionDirectory, $"AdventOfCode{year}");
            if (!Directory.Exists(dir))
            {
                throw new UnconfiguredException($"Project {dir} does not exist.");
            }

            var days = Directory.GetDirectories(dir, "Day*")
                                .Select(s => int.TryParse(s.Substring(s.Length - 2), out int day) ? day : 0)
                                .OrderByDescending(i => i);
            int newDay = days.First() + 1;

            if (newDay > 25)
            {
                throw new WarningException($"{year} is completed.");
            }

            string newDir = Path.Combine(dir, $"Day{newDay:00}");
            string newSource = Path.Combine(newDir, $"Day{newDay:00}.cs");
            string newTest = Path.Combine(newDir, $"Day{newDay:00}Tests.cs");
            Directory.CreateDirectory(newDir);
            File.Copy(Path.Combine(dir, "Day00", "Data.txt"), Path.Combine(newDir, "Data.txt"));
            File.Copy(Path.Combine(dir, "Day00", "Test.txt"), Path.Combine(newDir, "Test.txt"));
            File.Copy(Path.Combine(dir, "Day00", "Day00.cs"), newSource);
            File.Copy(Path.Combine(dir, "Day00", "Day00Tests.cs"), newTest);

            UpdateDayInFile(newSource, newDay);
            UpdateDayInFile(newTest, newDay);

            UpdateProjectFile(dir, year, newDay);

            return (dir, newDay);
        }

        static void UpdateDayInFile(string filename, int day)
        {
            string contents = File.ReadAllText(filename);
            contents = contents.Replace("00", $"{day:00}");
            File.WriteAllText(filename, contents);
        }

        static void UpdateProjectFile(string dir, int year, int day)
        {
            string file = Path.Combine(dir, $"AdventOfCode{year}.csproj");

            XDocument proj = XDocument.Load(file);
            XElement itemGroup = proj.Descendants("ItemGroup").Last();

            XElement data = new XElement("None",
                new XAttribute("Update", $@"Day{day:00}\Data.txt"),
                new XElement("CopyToOutputDirectory", "PreserveNewest"));
            itemGroup.Add(data);

            XElement test = new XElement("None",
                new XAttribute("Update", $@"Day{day:00}\Test.txt"),
                new XElement("CopyToOutputDirectory", "PreserveNewest"));
            itemGroup.Add(test);

            proj.Save(file);
        }

        public void RunTests(int year, int day)
        {
            string dir = Path.Combine(_configuration.SolutionDirectory, $"AdventOfCode{year}");
            if (!Directory.Exists(dir))
            {
                throw new UnconfiguredException($"Project {dir} does not exist.");
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
