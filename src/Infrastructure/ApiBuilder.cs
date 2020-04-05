using Core;
using System.Net.Http;

namespace Infrastructure
{
    public static class ApiBuilder
    {
        private static HttpClient apiClient;

        public static void SetClient(HttpClient client)
        {
            Guard.EnsureNotNull(client, nameof(client));

            apiClient = client;
        }

        public static HttpClient GetClient()
        {
            return apiClient;
        }
    }
}
