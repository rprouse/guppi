using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using Guppi.Core.Configurations;
using Guppi.Core.Exceptions;
using Guppi.Core.Services.AdventOfCode;
using Guppi.Core.Entities.AdventOfCode;
using Guppi.Core.Interfaces;

namespace Guppi.Core.Services;

internal sealed class AdventOfCodeService : IAdventOfCodeService
{
    private readonly IHttpRestProvider _restService;
    private readonly IProcessProvider _process;

    public AdventOfCodeService(IHttpRestProvider restService, IProcessProvider process)
    {
        _restService = restService;
        _process = process;
    }

    public void Configure()
    {
        var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        configuration.RunConfiguration("Advent of Code", "Enter the Advent of Code session cookie value.");
    }

    public async Task<Leaderboard> GetLeaderboard(int year, int board)
    {
        var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        if (!configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the Advent of Code provider.");
        }

        _restService.AddHeader("Cookie", $"session={configuration.LoginToken}");
        var leaders = await _restService.GetData<AocLeaderboard>($"https://adventofcode.com/{year}/leaderboard/private/view/{board}.json");
        return leaders.GetLeaderboard();
    }

    public AddDayDto AddDayTo(int year)
    {
        var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        string dir = Path.Combine(configuration.SolutionDirectory, $"AdventOfCode{year}");
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

        return new AddDayDto { Directory = dir, NewDay = newDay };
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

    public async Task<string> GetPuzzleData(int year, int day)
    {
        var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        if (!configuration.Configured)
        {
            throw new UnconfiguredException("Please configure the Advent of Code provider.");
        }

        _restService.AddHeader("Cookie", $"session={configuration.LoginToken}");
        return await _restService.GetStringAsync($"https://adventofcode.com/{year}/day/{day}/input");
    }

    public async Task AddPuzzleDataToProject(int year, int day, string data)
    {
        var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        string file = Path.Combine(configuration.SolutionDirectory, $"AdventOfCode{year}", $"Day{day:00}", "Data.txt");
        if (!File.Exists(file))
        {
            throw new WarningException($"Data file {file} does not exist.");
        }
        await File.WriteAllTextAsync(file, data);
    }

    public void RunTests(int day, int year)
    {
        var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
        string dir = Path.Combine(configuration.SolutionDirectory, $"AdventOfCode{year}");
        if (!Directory.Exists(dir))
        {
            throw new UnconfiguredException($"Project {dir} does not exist.");
        }
        _process.Start("dotnet", "test" + GetTestFilter(year, day), dir, true);
    }

    static string GetTestFilter(int year, int day) =>
        day == 0 ? "" : $" --filter FullyQualifiedName=AdventOfCode{year}.Day{day:00}Tests";

}
