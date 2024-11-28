using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Database.Entities;
using System.Data.Entity;

namespace Database.Repositories
{
    public class TweetRepository : Repository<Tweet>, ITweetRepository
    {
        Context _context;
        public TweetRepository(Context context) : base(context) 
        {
            _context = context;
        }

        public IEnumerable<Tweet> Search(string query)
        {
            return null;
        }
    }
}
