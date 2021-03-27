using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Guppi.Application.Configurations;
using Guppi.Application.Exceptions;
using MediatR;

namespace Guppi.Application.Commands.AdventOfCode
{
    public sealed class AddDayCommand : IRequest<AddDayDto>
    {
        public int Year { get; init; }
    }

    internal sealed class AddDayCommandHandler : IRequestHandler<AddDayCommand, AddDayDto>
    {
        public async Task<AddDayDto> Handle(AddDayCommand request, CancellationToken cancellationToken)
        {
            var configuration = Configuration.Load<AdventOfCodeConfiguration>("AdventOfCode");
            string dir = Path.Combine(configuration.SolutionDirectory, $"AdventOfCode{request.Year}");
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
                throw new WarningException($"{request.Year} is completed.");
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

            UpdateProjectFile(dir, request.Year, newDay);

            return await Task.FromResult(new AddDayDto { Directory = dir, NewDay = newDay });
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
    }
}
