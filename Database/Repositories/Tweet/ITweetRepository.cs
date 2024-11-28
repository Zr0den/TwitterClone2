using Database.Entities;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface ITweetRepository : IRepository<Tweet>
    {
        IEnumerable<Tweet> Search(string query);
    }
}
