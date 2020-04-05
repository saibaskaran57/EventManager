using Microsoft.AspNetCore.Mvc.Testing;
using Service.Integration.Tests.Steps;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Service.Integration.Tests
{
    public class EventTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly EventSteps steps;

        public EventTests(WebApplicationFactory<Startup> factory)
        {
            this.steps = new EventSteps(factory);
        }

        [Fact]
        public async Task ShouldSuccessfullyValidateEventReceived()
        {
            var eventId = Guid.NewGuid().ToString();

            await this.steps.GivenISetupService();
            await this.steps.WhenICreateSubscription();
            await this.steps.AndIReceiveEventFromSaaSProvider(eventId);
            await this.steps.ThenEventShouldExistViaWebhookUrl(eventId);
            await this.steps.ThenShouldRemoveSubscriptionSuccessully();
        }

        [Fact]
        public async Task ShouldSuccessfullyOverrideEvents()
        {
            var event1Id = Guid.NewGuid().ToString();
            var event2Id = Guid.NewGuid().ToString();

            await this.steps.GivenISetupService();
            await this.steps.WhenICreateSubscription();
            await this.steps.AndIReceiveEventFromSaaSProvider(event1Id);
            await this.steps.ThenEventShouldExistViaWebhookUrl(event1Id);
            await this.steps.AndIReceiveEventFromSaaSProvider(event2Id);
            await this.steps.ThenEventShouldExistViaWebhookUrl(event2Id);
            await this.steps.ThenShouldRemoveSubscriptionSuccessully();
        }
    }
}