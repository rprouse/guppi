using System.Net.Http;
using System.Threading.Tasks;

#nullable enable

namespace Guppi.Core.Interfaces
{
    public interface IHttpRestService
    {
        HttpClient Client { get; }

        Task<T?> GetData<T>(string url);

        void AddHeader(string name, string? value);

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);

        Task<string> GetStringAsync(string? requestUri);
    }
}
