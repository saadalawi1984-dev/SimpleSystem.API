using System.Threading.Tasks;
using SimpleSystem.DataAccess.Entities;

namespace SimpleSystem.DataAccess.Repositories.Interfaces
{
    public interface ICountryRepository : IGenaricRepository<Country>
    {
        // إذا كان لديك دالة خاصة بالدول فقط (مثل البحث باسم الدولة)، يمكنك إضافتها هنا:
        Task<Country?> GetByNameAsync(string countryName);
    }
}
