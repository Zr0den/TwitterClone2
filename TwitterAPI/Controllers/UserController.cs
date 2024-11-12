using Database.Entities;
using Helpers;
using MessageClient;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using SearchService;
using UserProfileService;


namespace TwitterAPI.Controllers
{
    //internal static class MessageWaiter
    //{
    //    public static async Task<T?>? WaitForMessage<T>(MessageClient<T> messageClient, string clientId, int timeout = 5000)
    //    {
    //        var tcs = new TaskCompletionSource<T?>();
    //        var cancellationTokenSource = new CancellationTokenSource(timeout);
    //        cancellationTokenSource.Token.Register(() => tcs.TrySetResult(default));

    //        using (
    //            var connection = messageClient.ListenUsingTopic<T>(message =>
    //            {
    //                tcs.TrySetResult(message);
    //            }, "User" + clientId, clientId)
    //        )
    //        {
    //        }

    //        return await tcs.Task;
    //    }
    //}

    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly SearchingService _searchService;
        private readonly MessageClient<UserCreateDto> _createUserMessage;
        private readonly MessageClient<UserProfileDto> _getUserMessage;

        public UserController(UserService userService, SearchingService searchService, MessageClient<UserCreateDto> createUserMessage, MessageClient<UserProfileDto> getUserMessage)
        {
            _userService = userService;
            _searchService = searchService;
            _createUserMessage = createUserMessage;
            _getUserMessage = getUserMessage;
        }
        [HttpPost]
        public async Task<ActionResult<string>> CreateUser(UserCreateDto userDto)
        {
            _createUserMessage.SendUsingTopic(new UserCreateDto
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password,
                UserTag = userDto.UserTag
            }, "newUser");


            var response = await MessageWaiter.WaitForMessage(_getUserMessage, userDto.UserTag)!;

            return response != null ? response.Email : "Order timed out.";
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, UserUpdateDto userDto)
        {
            var user = _searchService.GetUserById(id);
            if (user == null)
                return NotFound();

            user.Name = userDto.Name;
            user.UserTag = userDto.UserTag;
            user.Email = userDto.Email;

             _userService.UpdateUser(user);

            return NoContent();
        }
        // Get a user profile by ID
        [HttpGet("{id}")]
        public ActionResult<UserProfileDto> GetUserById(int id)
        {
            var user = _searchService.GetUserById(id);
            if (user == null)
                return NotFound();

            var result = new UserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                UserTag = user.UserTag,
                Email = user.Email
            };

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers(string query)
        {
            _getUserMessage.SendUsingTopic(new UserProfileDto
            {
                Query = query
            }, "getUsers");


            var response = await MessageWaiter.WaitForMessage(_getUserMessage, query)!;

            return response != null ? response.UserProfiles : new List<UserDto>();
        }

    }
}
