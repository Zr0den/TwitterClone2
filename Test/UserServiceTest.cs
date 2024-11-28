//using Database.Entities;
//using Database.Repositories;
//using Helpers;
//using MessageClient;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using System.Threading.Tasks;
//using Xunit;

namespace Test
{
    public class UserServiceTests
    {
        //private readonly Mock<IUserRepository> _userRepositoryMock;
        //private readonly Mock<MessageClient<UserCreateDto>> _newUserClientMock;
        //private readonly Mock<MessageClient<UserProfileDto>> _userProfileClientMock;
        //private readonly UserService _userService;
        //private readonly SearchingService _searchService;

        //public UserServiceTests()
        //{
        //    _userRepositoryMock = new Mock<IUserRepository>();
        //    _newUserClientMock = new Mock<MessageClient<UserCreateDto>>();
        //    _userProfileClientMock = new Mock<MessageClient<UserProfileDto>>();
        //    _userService = new UserService(_userRepositoryMock.Object, _newUserClientMock.Object, _userProfileClientMock.Object);
        //    _searchService = new SearchingService(_userProfileClientMock.Object);
        //}

    //    [Fact]
    //    public void GetUserByIdFindsUser()
    //    {
    //        // Arrange
    //        var id = 1;
    //        var expectedUser = new User { Id = id, Name = "Test Tester", UserTag = "Testerman" };

    //        _userRepositoryMock.Setup(repo => repo.GetById(id))
    //            .Returns(expectedUser);

    //        // Act
    //        var user = _searchService.GetUserById(id);

    //        // Assert
    //        Assert.NotNull(user);
    //        Assert.Equal(expectedUser.Id, user.Id);
    //        Assert.Equal(expectedUser.Name, user.Name);
    //    }

    //    [Fact]
    //    public void GetUserByIdReturnsNullWhenNotExists()
    //    {
    //        // Arrange
    //        var userId = 1;
    //        _userRepositoryMock.Setup(repo => repo.GetById(userId))
    //            .Returns((User)null);

    //        // Act
    //        var user = _searchService.GetUserById(userId);

    //        // Assert
    //        Assert.Null(user);
    //    }
    }
}