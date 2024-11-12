using Database;
using Database.Entities;
using Database.Repositories;
using Helpers;
using MessageClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserProfileService
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly MessageClient<UserCreateDto> _newUserClient;
        private readonly MessageClient<UserProfileDto> _userProfileClient;

        public UserService(IUserRepository userRepository, MessageClient<UserCreateDto> newUserClient, MessageClient<UserProfileDto> userProfileClient)
        {
            _userRepository = userRepository;
            _newUserClient = newUserClient;
            _userProfileClient = userProfileClient;
        }
        public void Start()
        {
            Action<UserCreateDto> callback = AddUser;
            _newUserClient.Connect();
            _newUserClient.ListenUsingTopic(callback, "", "newUser");

            Action<string> callback2 = GetAllUsers;
            _userProfileClient.Connect();
            _userProfileClient.ListenUsingTopic(callback, "", "getAllUsers");
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

            _userProfileClient.SendUsingTopic(dto, "sendUserList");
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
