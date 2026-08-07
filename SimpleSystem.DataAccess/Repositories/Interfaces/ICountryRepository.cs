using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        List<Country> GetAll();
        Country? GetById(int countryId);
        int Add(Country country);
        bool Update(Country country);
        bool Delete(int countryId);
    }
}