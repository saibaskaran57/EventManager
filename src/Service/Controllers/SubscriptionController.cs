using System;
using System.Threading.Tasks;
using Core;
using Infrastructure;
using Infrastructure.Models;
using Microsoft.AspNetCore.Mvc;
using Service.Constants;
using Service.Models;

namespace Service.Controllers
{
    [Route("event/api/[controller]")]
    [ApiController]
    public class SubscriptionController : ControllerBase
    {
        private readonly IRepository<Subscription> repository;
        private readonly SubscriptionClient client;

        public SubscriptionController(IRepository<Subscription> repository, SubscriptionClient client)
        {
            Guard.EnsureNotNull(repository, nameof(repository));
            Guard.EnsureNotNull(client, nameof(client));

            this.repository = repository;
            this.client = client;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SubscriptionRequest request)
        {
            var webhookId = Guid.NewGuid().ToString();
            var subscription = new Subscription { CallbackUrl = request.CallbackUrl };

            await this.repository.Save(webhookId, subscription).ConfigureAwait(false);

            return Ok(webhookId);
        }

        [HttpGet("{webhookId}")]
        public async Task<IActionResult> Get(
            [FromRoute(Name = QueryParams.WebhookId)] string webhookId)
        {
            var result = await this.repository.Retrieve(webhookId).ConfigureAwait(false);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("{webhookId}")]
        public async Task<IActionResult> Notify(
            [FromRoute(Name = QueryParams.WebhookId)] string webhookId,
            [FromBody] object body)
        {
            var result = await this.repository.Retrieve(webhookId).ConfigureAwait(false);

            var data = body != null 
                ? body.ToString() 
                : string.Empty;

            await this.client.Notify(result.CallbackUrl, data);

            return Ok();
        }

        [HttpDelete("{webhookId}")]
        public async Task<IActionResult> Delete(
            [FromRoute(Name = QueryParams.WebhookId)] string webhookId)
        {
            await this.repository.Delete(webhookId).ConfigureAwait(false);

            return Ok();
        }
    }
}