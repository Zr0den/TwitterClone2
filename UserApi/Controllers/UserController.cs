using Database.Entities;
using Database.Repositories;
using Helpers;
using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace UserApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        private readonly RabbitMQConsumer _consumer;
        private UserService _service;
        private IUserRepository _userRepository;

        public UserController(RabbitMQProducer producer, RabbitMQConsumer consumer, UserService service, IUserRepository userRepository)
        {
            _producer = producer;
            _consumer = consumer;
            _service = service;
            _userRepository = userRepository;

            Start();
        }

        public async void Start()
        {
            await _consumer.StartListeningAsync("search-api-response-queue");
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchUsers([FromQuery] string query)
        {
            var users = _userRepository.GetAll();

            var filteredUsers = users.Where(u => u.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            var message = JsonSerializer.Serialize(filteredUsers);

            await _producer.PublishAsync(message);

            return Ok(filteredUsers);
        }
    }
}
