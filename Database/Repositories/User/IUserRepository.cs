using Database.Entities;
using System.Threading.Tasks;

namespace Database.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<IEnumerable<User>> SearchAsync(string query);
    }
}
