using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

namespace Guppi.Infrastructure.Services
{
    public abstract class HttpService
    {
        protected HttpClient Client { get; }

        public HttpService()
        {
            Client = new HttpClient();
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            Client.DefaultRequestHeaders.Add("User-Agent", "Guppi CLI (https://github.com/rprouse/guppi)");
        }

        protected async Task<T> GetData<T>(string url)
        {
            string json = await Client.GetStringAsync(url);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
