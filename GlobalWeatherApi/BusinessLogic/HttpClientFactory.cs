using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading.Tasks;

namespace GlobalWeatherApi.BusinessLogic
{
    public interface IHttpClientFactory
    {
        IHttpClientWrapper CreateHttpClient();
    }

    [ExcludeFromCodeCoverage]
    public class HttpClientFactory : IHttpClientFactory
    {
        public IHttpClientWrapper CreateHttpClient()
        {
            return new HttpClientWrapper(new HttpClient());
        }
    }

    public interface IHttpClientWrapper : IDisposable
    {
        Task<HttpResponseMessage> GetAsync(string requestUri);
    }

    [ExcludeFromCodeCoverage]
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public  Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            return _httpClient.GetAsync(requestUri);
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
        }
    }
}