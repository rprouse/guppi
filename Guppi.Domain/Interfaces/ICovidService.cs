using System.Threading.Tasks;
using Guppi.Domain.Entities.Covid;

namespace Guppi.Domain.Interfaces
{
    public interface ICovidService
    {
        Task<CovidData> GetCovidData(Country country);
    }
}
