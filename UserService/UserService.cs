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

        public UserService(IUserRepository userRepository, MessageClient<UserCreateDto> newUserClient)
        {
            _userRepository = userRepository;
            _newUserClient = newUserClient;
        }
        public void Start()
        {
            Action<UserCreateDto> callback = AddUserAsync;
            _newUserClient.Connect();
            _newUserClient.ListenUsingTopic(callback, "", "newUser");

        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public void AddUserAsync(UserCreateDto userDto)
        {
            var user = new User
            {
                Name = userDto.Name,
                UserTag = userDto.UserTag,
                Email = userDto.Email,
                Password = userDto.Password,
                // Password handling should ideally involve hashing
            };
            _userRepository.AddAsync(user);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(int id)
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}
