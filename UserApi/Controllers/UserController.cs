using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        private UserService _service;
        public UserController(RabbitMQProducer producer, UserService service)
        {
            _producer = producer;
            _service = service;
        }

        [HttpPost("process")]
        public IActionResult ProcessRequest([FromBody] RequestDetails requestDetails)
        {
            return Ok($"TEST Processed {requestDetails.Method} request for {requestDetails.Url}");
        }
    }
}
