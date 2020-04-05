using Core;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Service.Models;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Service.Integration.Tests.Steps
{
    public sealed class EventSteps
    {
        private readonly string eventServiceEndpoint;
        private readonly string subscriptionServiceEndpoint;
        private readonly HttpClient client;

        private string notificationId;
        private string webhookId;

        public EventSteps(WebApplicationFactory<Startup> factory)
        {
            this.eventServiceEndpoint = TestOptions.EventServiceEndpoint;
            this.subscriptionServiceEndpoint = TestOptions.SubscriptionServiceEndpoint;
            this.client = factory.CreateClient();

            ApiBuilder.SetClient(this.client);
        }

        public async Task<EventSteps> GivenISetupService()
        {
            this.notificationId = Guid.NewGuid().ToString();

            return await Task.FromResult(this);
        }

        public async Task<EventSteps> WhenICreateSubscription()
        {
            var request = new SubscriptionRequest
            {
                CallbackUrl = $"{eventServiceEndpoint}/{this.notificationId}"
            };

            var flattenedRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(flattenedRequest, Encoding.UTF8, MediaType.Json);

            var response = await client.PostAsync(subscriptionServiceEndpoint, content).ConfigureAwait(false);
            this.webhookId = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return await Task.FromResult(this);
        }

        public async Task AndIReceiveEventFromSaaSProvider(string eventId)
        {
            var request = new Event { EventId = eventId };
            var flattenedRequest = JsonConvert.SerializeObject(request);
            var content = new StringContent(flattenedRequest, Encoding.UTF8, MediaType.Json);

            await client.PostAsync($"{subscriptionServiceEndpoint}/{this.webhookId}", content).ConfigureAwait(false);
        }

        public async Task ThenEventShouldExistViaWebhookUrl(string eventId)
        {
            var response = await client.GetAsync($"{eventServiceEndpoint}/{this.notificationId}").ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<Event>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            Assert.Equal(eventId, result.EventId);
        }

        public async Task ThenShouldRemoveSubscriptionSuccessully()
        {
            await client.DeleteAsync($"{subscriptionServiceEndpoint}/{this.webhookId}").ConfigureAwait(false);

            var response = await client.GetAsync($"{subscriptionServiceEndpoint}/{this.webhookId}").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
