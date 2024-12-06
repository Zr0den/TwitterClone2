using Helpers;
using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Net.Http.Headers;
using System.Text.Json;

namespace SearchApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        private readonly RabbitMQConsumer _consumer;
        private SearchService _service;
        private readonly SecretSettings _secretSettings;

        public SearchController(RabbitMQProducer producer, RabbitMQConsumer consumer, SearchService service, SecretSettings secretSettings)
        {
            _producer = producer;
            _consumer = consumer;
            _service = service;
            _secretSettings = secretSettings;
        }



        [HttpGet("user")]
        public async Task<IActionResult> SearchUsersAsync([FromQuery] string query)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _secretSettings.TWITTER_SERVICE_TOKEN);
            var requestDetails = new RequestDetails
            {
                Method = Method.Get,
                Url = $"http://userapi/user/search?query={query}", //TODO
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
