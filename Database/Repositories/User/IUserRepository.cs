using Database.Entities;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        IEnumerable<User> Search(string query);
    }
}
