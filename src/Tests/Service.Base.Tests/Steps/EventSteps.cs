﻿using Core;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Service.Constants;
using Service.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Service.Base.Tests.Steps
{
    public sealed class EventSteps
    {
        private readonly string eventServiceEndpoint;
        private readonly string eventServiceKey;
        private readonly string subscriptionServiceEndpoint;
        private readonly HttpClient client;

        private string notificationId;
        private string webhookId;

        public EventSteps(HttpClient client, TestOption option)
        {
            this.eventServiceEndpoint = option.EventServiceEndpoint;
            this.eventServiceKey = option.EventServiceKey;
            this.subscriptionServiceEndpoint = option.SubscriptionServiceEndpoint;
            this.client = client;

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
                CallbackUrl = AddServiceKey($"{eventServiceEndpoint}/{this.notificationId}")
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
            var endpoint = AddServiceKey($"{eventServiceEndpoint}/{this.notificationId}");
            var response = await client.GetAsync(endpoint).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<Event>(await response.Content.ReadAsStringAsync().ConfigureAwait(false));

            Assert.True(response.IsSuccessStatusCode);
            Assert.Equal(eventId, result.EventId);
        }

        public async Task ThenShouldRemoveSubscriptionSuccessully()
        {
            await client.DeleteAsync($"{subscriptionServiceEndpoint}/{this.webhookId}").ConfigureAwait(false);

            var response = await client.GetAsync($"{subscriptionServiceEndpoint}/{this.webhookId}").ConfigureAwait(false);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        private string AddServiceKey(string endpoint)
        {
            var queryString = new Dictionary<string, string>
            {
                { QueryParams.AccessKey, this.eventServiceKey }
            };

            return QueryHelpers.AddQueryString(endpoint, queryString);
        }
    }
}
