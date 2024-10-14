using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Guppi.Core.Interfaces;

#nullable enable

namespace Guppi.Core.Providers;

public class HttpRestProvider(HttpClient client) : IHttpRestProvider
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
