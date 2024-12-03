using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    public class TestResponse
    {
        public string Response { get; set; }

    }
    [ApiController]
    [Route("auth")]
    public class AuthServiceController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthServiceController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpGet]
        public TestResponse test()
        {
            return new TestResponse { Response = "test" };
        }

        [HttpPost]
        public IActionResult Login()
        {
            var token = _jwtTokenService.CreateToken();
            return Ok(token);
        }
    }
}
