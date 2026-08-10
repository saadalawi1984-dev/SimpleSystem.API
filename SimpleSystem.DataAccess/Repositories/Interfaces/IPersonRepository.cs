using System.Threading.Tasks;
using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface IPersonRepository : IGenaricRepository<Person>
    {
       
        Task<Person?> GetByNationalNoAsync(string nationalNo);
    }
}
