using System.Threading.Tasks;
using Guppi.Domain.Entities.Covid;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Covid
{
    public class CovidService : HttpService, ICovidService
    {
        public async Task<CovidData> GetCovidData(Country country)
        {
            var data = await GetData<NyTimesCovidData>($"https://static01.nyt.com/newsgraphics/2020/03/16/coronavirus-maps/f6f3e99e94435fad8987e9cc5e56436811b62995/data/timeseries/en/{country}.json");
            return data.GetCovidData();
        }
    }
}
