using Database.Entities;
using Database.Repositories;
using Helpers;

namespace UserApi
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void GetAllUsers(string query)
        {
            var users = _userRepository.GetAll();
            UserProfileDto dto = new UserProfileDto();
            dto.UserProfiles = new List<UserDto>();
            dto.Query = query;

            foreach (var user in users)
            {
                dto.UserProfiles.Add(new UserDto { Id = user.Id, Email = user.Email, Name = user.Name, UserTag = user.UserTag });
            }

            //_userProfileClient.SendUsingTopic(dto, "sendUserList");
        }

        public void AddUser(UserCreateDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                UserTag = userDto.UserTag,
                Email = userDto.Email,
                Password = userDto.Password,
                // Password handling should ideally involve hashing
            };
            _userRepository.Add(user);
        }

        public void UpdateUser(User user)
        {
            _userRepository.Update(user);
        }

        public void DeleteUser(int id)
        {
            _userRepository.Delete(id);
        }
    }
}
