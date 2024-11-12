using Database.Entities;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using TwitterAPI.Messaging;
using UserProfileService;


namespace TwitterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [HttpPost]
        public async Task<ActionResult<UserProfileDto>> CreateUser(UserCreateDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                UserTag = userDto.Username,
                Email = userDto.Email,
                Password = userDto.Password,
                // Password handling should ideally involve hashing
            };

            await _userService.AddUserAsync(user);

            // Mapping the User entity to UserProfileDto
            var result = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.UserTag,
                Email = user.Email
            };

            return CreatedAtAction(nameof(_userService.GetUserByIdAsync), new { id = result.Id }, result);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserUpdateDto userDto)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            user.Name = userDto.Name;
            user.UserTag = userDto.Username;
            user.Email = userDto.Email;

            await _userService.UpdateUserAsync(user);

            return NoContent();
        }
        // Get a user profile by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UserProfileDto>> GetUserById(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
                return NotFound();

            var result = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.UserTag,
                Email = user.Email
            };

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserProfileDto>>> SearchUsers(string query)
        {
            var users = await _userService.SearchAsync(query);

            var result = users.Select(user => new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Username = user.UserTag,
                Email = user.Email
            }).ToList();

            return Ok(result);
        }

    }
}
