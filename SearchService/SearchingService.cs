using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Entities;
using Database.Repositories;
using Helpers;
using MessageClient;

namespace SearchService
{
    public class SearchingService
    {
        private readonly MessageClient<UserProfileDto> _userProfileClient;


        public SearchingService(MessageClient<UserProfileDto> userProfileClient)
        {
            _userProfileClient = userProfileClient;
        }

        public void Start()
        {
            Action<string> callback = Search;
            _userProfileClient.Connect();
            _userProfileClient.ListenUsingTopic(callback, "", "getUsers");

            //Action<IEnumerable<User>> callback2 = Search;
            //_userProfileClient.Connect();
            //_userProfileClient.ListenUsingTopic(callback, "", "sendUserList");
        }

        public User GetUserById(int id)
        {
            //return await _userRepository.GetByIdAsync(id);
            return null;
        }

        public async Task<IEnumerable<UserDto>> Search(string query)
        {
            //return await _userRepository.SearchAsync(query);

            _userProfileClient.SendUsingTopic(new UserProfileDto
            {
                Query = query
            }, "getAllUsers");

            UserProfileDto response = await MessageWaiter.WaitForMessage(_userProfileClient, query)!;

            List<UserDto> users = new List<UserDto>();
            foreach (var profile in response.UserProfiles)
            {
                users.Add(new UserDto { Id = profile.Id, Name = profile.Name, Email = profile.Email, UserTag = profile.UserTag });
            }

            return users;
            //_userProfileClient.SendUsingTopic(query, "getUsers");
        }

        public IEnumerable<User> Search(UserCreateDto dto) 
        {
            return null;
        }

    }
}
