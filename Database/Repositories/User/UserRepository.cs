using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Database.Entities;
using System.Data.Entity;

namespace Database.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        Context _context;
        public UserRepository(Context context) : base(context) 
        {
            _context = context;
        }

        public IEnumerable<User> Search(string query)
        {
            return _context.Users
                .Where(u => u.Name.Contains(query) || u.UserTag.Contains(query))
                .ToList();
        }
    }
}
