using Helpers;
using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Text.Json;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        private readonly RabbitMQConsumer _consumer;
        private SearchService _service;

        public SearchController(RabbitMQProducer producer, RabbitMQConsumer consumer, SearchService service)
        {
            _producer = producer;
            _consumer = consumer;
            _service = service;
        }

        [HttpGet("search-users")]
        public async Task<IActionResult> SearchUsersAsync([FromQuery] string query)
        {
            var requestDetails = new RequestDetails
            {
                Method = Method.Get,
                Url = $"http://userapi-url/user/search?query={query}", //TODO
                ReplyToQueue = "search-api-response-queue"
            };

            var message = JsonSerializer.Serialize(requestDetails);
            await _producer.PublishAsync(message);

            var response = await _consumer.WaitForResponseAsync("search-api-response-queue");
            var users = JsonSerializer.Deserialize<List<UserDto>>(response);

            return Ok(users);
        }
    }
}
