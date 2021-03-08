using System.Threading.Tasks;
using Guppi.Domain.Entities.Covid;
using Guppi.Domain.Interfaces;

namespace Guppi.Infrastructure.Services.Covid
{
    public class CovidService : HttpService, ICovidService
    {
        const string ROOT_URI = "https://static01.nyt.com/newsgraphics/2020/03/16/coronavirus-maps/";

        public async Task<CovidData> GetCovidData(Country country)
        {
            var code = await GetCode();
            var data = await GetData<NyTimesCovidData>($"{ROOT_URI}{code}/data/timeseries/en/{country}.json");
            return data.GetCovidData();
        }

        /// <summary>
        /// Gets the code for today's covid data
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCode()
        {
            // Load the Covid page and search it for a link to to the data we want
            string html = await Client.GetStringAsync("https://www.nytimes.com/interactive/2020/us/coronavirus-us-cases.html");
            int start = html.IndexOf(ROOT_URI) + ROOT_URI.Length;
            int end = html.IndexOf('/', start + 1);
            string code = html.Substring(start, end - start);
            return code;
        }
    }
}
