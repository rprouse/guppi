using System.Threading.Tasks;
using Guppi.Domain.Entities.AdventOfCode;

namespace Guppi.Domain.Interfaces
{
    public interface IAdventOfCodeService
    {
        Task<Leaderboard> GetLeaderboard(int year, int board);

        (string directory, int newDay) AddDayTo(int year);

        void RunTests(int year, int day);

        void Configure();
    }
}
