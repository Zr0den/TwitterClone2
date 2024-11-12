using Database.Entities;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Moq;
using SearchService;
using System.Threading.Tasks;
using TwitterAPI.Controllers;
using UserProfileService;
using Xunit;

namespace Test
{
    public class UserControllerTests
    {
        private readonly Mock<UserService> _userServiceMock;
        private readonly Mock<SearchingService> _searchServiceMock;
        private readonly UserController _controller;

        public UserControllerTests()
        {
            _userServiceMock = new Mock<UserService>();
            _searchServiceMock = new Mock<SearchingService>();
            _controller = new UserController(_userServiceMock.Object, _searchServiceMock.Object);
        }

        [Fact]
        public async Task CreateUserActionResultTest()
        {
            // Arrange
            var userDto = new UserCreateDto { Name = "John Tester", Username = "Testerman", Email = "test@example.com" };
            var createdUser = new User { Id = 1, Name = "John Tester", UserTag = "Testerman", Email = "test@example.com" };

            _userServiceMock.Setup(s => s.AddUserAsync(It.IsAny<User>()))
                .Callback<User>(user => user.Id = 1)
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.CreateUser(userDto);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            var userProfile = Assert.IsType<UserProfileDto>(actionResult.Value);
            Assert.Equal(createdUser.Id, userProfile.Id);
            Assert.Equal(createdUser.Name, userProfile.Name);
        }

        [Fact]
        public async Task SearchUsers_ShouldReturnOkResult_WithListOfUsers()
        {
            // Arrange
            var query = "Bobby";
            var users = new List<User>
            {
                new User { Id = 1, Name = "Bobby", UserTag = "Cool123", Email = "Bobby@example.com" },
                new User { Id = 2, Name = "Bobby Jr", UserTag = "Bob", Email = "Bobjr@example.com" }
            };

            _searchServiceMock.Setup(s => s.SearchAsync(query)).ReturnsAsync(users);

            // Act
            var result = await _controller.SearchUsers(query);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var userProfiles = Assert.IsType<List<UserProfileDto>>(okResult.Value);
            Assert.Equal(2, userProfiles.Count);
        }
    }
}
