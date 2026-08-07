using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository
    {
        List<User> GetAll();
        User? GetById(int userId);
        User? GetByPersonId(int personId);
        int Add(User user);
        bool Update(User user);
        bool Delete(int userId);
    }
}