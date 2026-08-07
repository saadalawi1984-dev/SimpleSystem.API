using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository: IGenaricRepository
    {
        User? GetByPersonId(int personId);
    }
}
