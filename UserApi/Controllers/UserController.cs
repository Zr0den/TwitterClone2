using Database.Entities;
using Database.Repositories;
using Helpers;
using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        private UserService _service;
        private IUserRepository _userRepository;

        public UserController(RabbitMQProducer producer, UserService service, IUserRepository userRepository)
        {
            _producer = producer;
            _service = service;
            _userRepository = userRepository;
        }

        [HttpGet("search")]
        public IActionResult SearchUsers([FromQuery] string query)
        {
            var users = _userRepository.GetAll();

            // var filteredUsers = users.Where(u => u.Name.Contains(query, StringComparison.OrdinalIgnoreCase)).ToList();
            var filteredUsers = new List<User>();
            User user = new User()
            {
                UserTag = "bodil",
                Email = "hansbodil",
                Name = "karl",
                Password = "1234",
                Id = 1005,
            };
            filteredUsers.Add(user);
            return Ok(filteredUsers);
        }
    }
}
