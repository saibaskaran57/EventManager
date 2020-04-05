using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Service.Constants;
using Service.Models;

namespace Service.Controllers
{
    [Route("/event/api/callback")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IRepository<object> repository;
        private readonly IOptions<ConfigurationOptions> options;

        public EventController(IRepository<object> repository, IOptions<ConfigurationOptions> options)
        {
            Guard.EnsureNotNull(repository, nameof(repository));
            Guard.EnsureNotNull(options, nameof(options));

            this.repository = repository;
            this.options = options;
        }

        [HttpPost("{notificationId}")]
        public async Task<IActionResult> Post(
            [FromRoute(Name = QueryParams.NotificationId)] string notificationId,
            [FromQuery(Name = QueryParams.AccessKey)] string accessKey,
            [FromQuery(Name = QueryParams.ValidationToken)] string validationToken,
            [FromBody] object body)
        {
            if(!string.IsNullOrWhiteSpace(validationToken))
            {
                return Ok(validationToken);
            }

            if(accessKey != options.Value.Key)
            {
                return Unauthorized();
            }

            await repository.Save(notificationId, body).ConfigureAwait(false);

            return Ok(notificationId);
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> Get(
            [FromRoute(Name = QueryParams.NotificationId)] string notificationId,
            [FromQuery(Name = QueryParams.AccessKey)] string accessKey)
        {
            if (accessKey != options.Value.Key)
            {
                return Unauthorized();
            }

            var result = await repository.Retrieve(notificationId).ConfigureAwait(false);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}