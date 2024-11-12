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

        public async Task<IEnumerable<User>> SearchAsync(string query)
        {
            return await _context.Users
                .Where(u => u.Name.Contains(query) || u.UserTag.Contains(query))
                .ToListAsync();
        }
    }
}
