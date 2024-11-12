using Database.Entities;
using Database.Repositories;
using Helpers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using UserProfileService;
using Xunit;

namespace Test
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userService = new UserService(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetUserByIdAsyncFindsUser()
        {
            // Arrange
            var id = 1;
            var expectedUser = new User { Id = id, Name = "Test Tester", UserTag = "Testerman" };

            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(expectedUser);

            // Act
            var user = await _userService.GetUserByIdAsync(id);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(expectedUser.Id, user.Id);
            Assert.Equal(expectedUser.Name, user.Name);
        }

        [Fact]
        public async Task GetUserByIdAsyncReturnsNullWhenNotExists()
        {
            // Arrange
            var userId = 1;
            _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId))
                .ReturnsAsync((User)null);

            // Act
            var user = await _userService.GetUserByIdAsync(userId);

            // Assert
            Assert.Null(user);
        }
    }
}