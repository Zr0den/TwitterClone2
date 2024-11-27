using AuthService.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthServiceController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;

        public AuthServiceController(JwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        public IActionResult Login()
        {
            var token = _jwtTokenService.CreateToken();
            return Ok(token);
        }
    }
}
