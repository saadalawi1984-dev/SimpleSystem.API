using System.Threading.Tasks;
using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IGenaricRepository<User>
    {
        Task<User?> GetByPersonIdAsync(int personId);
        Task<User?> GetByUsernameAsync(string username);
    }
}
