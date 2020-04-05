using Core;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public sealed class SubscriptionClient
    {
        private readonly IHttpClientFactory factory;

        public SubscriptionClient(IHttpClientFactory factory)
        {
            Guard.EnsureNotNull(factory, nameof(factory));

            this.factory = factory;
        }

        public async Task Notify(string callbackUrl, string body)
        {
            var client = ApiBuilder.GetClient() ?? factory.CreateClient();
            var content = new StringContent(body, Encoding.UTF8, MediaType.Json);

            await client.PostAsync(callbackUrl, content);
        }
    }
}
