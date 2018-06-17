using System.Net.Http;

namespace HelloWorldValidator
{
    public static class HttpClientFactoryExtensions
    {
        public static HttpClient CreateClient<TClient>(this IHttpClientFactory httpClientFactory)
        {
            return httpClientFactory.CreateClient(typeof(TClient).Name);
        }
    }
}
