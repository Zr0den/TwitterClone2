using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Database.Entities;
using Database.Repositories;

namespace SearchService
{
    public class SearchingService
    {
        private readonly IUserRepository _userRepository;

        public SearchingService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<User>> SearchAsync(string query)
        {
            return await _userRepository.SearchAsync(query);
        }
    }
}
