using System.Threading;
using System.Threading.Tasks;
using Guppi.Domain.Entities.Covid;
using Guppi.Domain.Interfaces;
using MediatR;

namespace Guppi.Application.Queries.Covid
{
    public sealed class CovidCountryDataQuery : IRequest<CovidData>
    {
        public Country Country { get; init; }
    }

    internal sealed class CovidCountryDataQueryHandler : IRequestHandler<CovidCountryDataQuery, CovidData>
    {
        const string ROOT_URI = "https://static01.nyt.com/newsgraphics/2020/03/16/coronavirus-maps/";

        private readonly IHttpRestService _restService;

        public CovidCountryDataQueryHandler(IHttpRestService restService)
        {
            _restService = restService;
        }

        public async Task<CovidData> Handle(CovidCountryDataQuery request, CancellationToken cancellationToken)
        {
            var code = await GetCode();
            var data = await _restService.GetData<NyTimesCovidData>($"{ROOT_URI}{code}/data/timeseries/en/{request.Country}.json");
            return data.GetCovidData();
        }

        /// <summary>
        /// Gets the code for today's covid data
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetCode()
        {
            // Load the Covid page and search it for a link to to the data we want
            string html = await _restService.Client.GetStringAsync("https://www.nytimes.com/interactive/2020/us/coronavirus-us-cases.html");
            int start = html.IndexOf(ROOT_URI) + ROOT_URI.Length;
            int end = html.IndexOf('/', start + 1);
            string code = html.Substring(start, end - start);
            return code;
        }
    }
}
