using Database.Entities;
using Database.Repositories;
using Helpers;

namespace SearchApi
{
    public class SearchService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITweetRepository _tweetRepository;

        public SearchService(IUserRepository userRepository, ITweetRepository tweetRepository)
        {
            _userRepository = userRepository;
            _tweetRepository = tweetRepository;
        }


        public void Start()
        {
            //Action<IEnumerable<User>> callback2 = Search;
            //_userProfileClient.Connect();
            //_userProfileClient.ListenUsingTopic(callback, "", "sendUserList");
        }

        public User GetUserById(int id)
        {
            return _userRepository.GetById(id);
        }

        public async Task<IEnumerable<UserDto>> Search(string query)
        {
            //TODO
            return null;
            //return await _userRepository.SearchAsync(query);

            //_userProfileClient.SendUsingTopic(new UserProfileDto
            //{
            //    Query = query
            //}, "getAllUsers");

            //UserProfileDto response = await MessageWaiter.WaitForMessage(_userProfileClient, query)!;

            //List<UserDto> users = new List<UserDto>();
            //foreach (var profile in response.UserProfiles)
            //{
            //    users.Add(new UserDto { Id = profile.Id, Name = profile.Name, Email = profile.Email, UserTag = profile.UserTag });
            //}

            //return users;
            //_userProfileClient.SendUsingTopic(query, "getUsers");
        }

        public IEnumerable<User> Search(UserCreateDto dto)
        {
            return null;
        }
    }
}
