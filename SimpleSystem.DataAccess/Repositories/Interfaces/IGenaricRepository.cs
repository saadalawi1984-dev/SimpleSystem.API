using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface IGenaricRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int entityId);
        Task<int> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int entityId);
    }
}
