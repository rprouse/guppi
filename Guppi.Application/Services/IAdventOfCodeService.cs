using System.Threading.Tasks;
using Guppi.Application.Services.AdventOfCode;
using Guppi.Domain.Entities.AdventOfCode;

namespace Guppi.Application.Services;

public interface IAdventOfCodeService
{
    void Configure();
    Task<Leaderboard> GetLeaderboard(int year, int board);
    AddDayDto AddDayTo(int year);
    Task<string> GetPuzzleData(int year, int day);
    Task AddPuzzleDataToProject(int year, int day, string data);
    void RunTests(int day, int year);
}
