//using Database.Entities;
//using Helpers;
//using MessageClient;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Razor.TagHelpers;
//using Moq;
//using System.Threading.Tasks;
//using Xunit;

namespace Test
{
    public class UserControllerTests
    {
        //private readonly Mock<UserService> _userServiceMock;
        //private readonly Mock<SearchingService> _searchServiceMock;
        //private readonly Mock<MessageClient<UserCreateDto>> _createUserMessageMock;
        //private readonly Mock<MessageClient<UserProfileDto>> _getUserMessageMock;
        //private readonly UserController _controller;

        //public UserControllerTests()
        //{
        //    _userServiceMock = new Mock<UserService>();
        //    _searchServiceMock = new Mock<SearchingService>();
        //    _createUserMessageMock = new Mock<MessageClient<UserCreateDto>>();
        //    _getUserMessageMock = new Mock<MessageClient<UserProfileDto>>();
        //    _controller = new UserController(_userServiceMock.Object, _searchServiceMock.Object, _createUserMessageMock.Object, _getUserMessageMock.Object);
        //}

        //[Fact]
        //public void SearchUsersFindsListTest()
        //{
        //    // Arrange
        //    var query = "Bobby";
        //    var users = new List<User>
        //    {
        //        new User { Id = 1, Name = "Bobby", UserTag = "Cool123", Email = "Bobby@example.com" },
        //        new User { Id = 2, Name = "Bobby Jr", UserTag = "Bob", Email = "Bobjr@example.com" }
        //    };

        //    _searchServiceMock.Setup(s => s.Search(query)).Returns(users);

        //    // Act
        //    var result = _controller.SearchUsers(query);

        //    // Assert
        //    var okResult = Assert.IsType<OkObjectResult>(result.Result);
        //    var userProfiles = Assert.IsType<List<UserProfileDto>>(okResult.Value);
        //    Assert.Equal(2, userProfiles.Count);
        //}
    }
}
