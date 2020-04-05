using System.Threading.Tasks;
using Core;
using Microsoft.AspNetCore.Mvc;
using Service.Constants;

namespace Service.Controllers
{
    [Route("/event/api/callback")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly IRepository<object> repository;

        public EventController(IRepository<object> repository)
        {
            Guard.EnsureNotNull(repository, nameof(repository));

            this.repository = repository;
        }

        [HttpPost("{notificationId}")]
        public async Task<IActionResult> Post(
            [FromRoute(Name = QueryParams.NotificationId)] string notificationId,
            [FromQuery(Name = QueryParams.ValidationToken)] string token,
            [FromBody] object body)
        {
            if(!string.IsNullOrWhiteSpace(token))
            {
                return Ok(token);
            }

            await repository.Save(notificationId, body).ConfigureAwait(false);

            return Ok(notificationId);
        }

        [HttpGet("{notificationId}")]
        public async Task<IActionResult> Get(string notificationId)
        {
            var result = await repository.Retrieve(notificationId).ConfigureAwait(false);

            if(result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}