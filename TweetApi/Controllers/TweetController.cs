using Helpers.RabbitMQ;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TweetApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly RabbitMQProducer _producer;
        public TweetController(RabbitMQProducer producer)
        {
            _producer = producer;
        }
    }
}
