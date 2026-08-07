using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface IPersonRepository
    {
        List<Person> GetAll();
        Person? GetById(int personId);
        int Add(Person person);
        bool Update(Person person);
        bool Delete(int personId);
    }
}