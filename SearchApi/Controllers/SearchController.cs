using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        private SearchService _service;
        public SearchController(RabbitMQProducer producer, SearchService service)
        {
            _producer = producer;
            _service = service;
        }

        [HttpPost("search-user")]
        public async Task<IActionResult> SearchUserAsync([FromBody] RequestDetails requestDetails)
        {
            var message = JsonSerializer.Serialize(requestDetails);
            await _producer.PublishAsync(message);
            return Ok("TEST Request sent to UserApi.");
        }
    }
}
