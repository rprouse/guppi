using System.IO;
using System.Linq;
using System.Xml.Linq;
using ColoredConsole;

namespace DataProvider.AdventOfCode
{
    internal class SolutionService
    {
        AdventOfCodeConfiguration _configuration;

        public SolutionService(AdventOfCodeConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void AddDayTo(int year)
        {
            string dir = Path.Combine(_configuration.SolutionDirectory, $"AdventOfCode{year}");
            if (!Directory.Exists(dir))
            {
                ColorConsole.WriteLine($"Project {dir} does not exist.".Red());
                ColorConsole.WriteLine("Configure the data provider to set the solution directory.".Cyan());
                return;
            }

            ColorConsole.WriteLine($"Adding new day to {dir}".Yellow());

            var days = Directory.GetDirectories(dir, "Day*")
                                .Select(s => int.TryParse(s.Substring(s.Length - 2), out int day) ? day : 0)
                                .OrderByDescending(i => i);
            int newDay = days.First() + 1;

            if(newDay > 25)
            {
                ColorConsole.WriteLine($"{year} is completed.".Yellow());
                ColorConsole.WriteLine("No day added.".Cyan());
                return;
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

            ColorConsole.WriteLine($"Added Day{newDay:00}".Cyan());
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
