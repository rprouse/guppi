using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Domain.Interfaces;

#nullable enable

namespace Guppi.Infrastructure.Services
{
    public class HttpRestService(HttpClient client) : IHttpRestService
    {
        public HttpClient Client { get; } = client;

        public async Task<T?> GetData<T>(string url)
        {
            string json = await Client.GetStringAsync(url);
            return JsonSerializer.Deserialize<T>(json);
        }

        public void AddHeader(string name, string? value) =>
            Client.DefaultRequestHeaders.Add(name, value);

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) =>
            Client.SendAsync(request);

        public Task<string> GetStringAsync(string? requestUri) => 
            Client.GetStringAsync(requestUri);
    }
}
