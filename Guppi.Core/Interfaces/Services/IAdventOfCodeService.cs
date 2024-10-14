using System.Threading.Tasks;
using Guppi.Core.Services.AdventOfCode;
using Guppi.Core.Entities.AdventOfCode;

namespace Guppi.Core.Interfaces.Services;

public interface IAdventOfCodeService
{
    void Configure();
    Task<Leaderboard> GetLeaderboard(int year, int board);
    AddDayDto AddDayTo(int year);
    Task<string> GetPuzzleData(int year, int day);
    Task AddPuzzleDataToProject(int year, int day, string data);
    void RunTests(int day, int year);
}
