using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSystem.Business
{
    public class CountryBusiness
    {
        private readonly ICountryRepository _countryRepo;

        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;

        // Constructor للتنفيذ والـ Dependency Injection
        public CountryBusiness(ICountryRepository? countryRepo = null)
        {
            _countryRepo = countryRepo ?? new CountryRepository();
        }

        private CountryBusiness(Country entity, enMode mode, ICountryRepository? countryRepo = null)
        {
            _countryRepo = countryRepo ?? new CountryRepository();
            this.CountryId = entity.CountryId;
            this.CountryName = entity.CountryName;
            this.CountryCode = entity.CountryCode;
            this.Mode = mode;
        }

        public Country ToEntity()
        {
            return new Country
            {
                CountryId = this.CountryId,
                CountryName = this.CountryName,
                CountryCode = this.CountryCode
            };
        }

        // 1. جلب كافة الدول Async
        public static async Task<List<Country>> GetAllCountriesAsync(ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            return await r.GetAllAsync();
        }

        // 2. البحث برقم الدولة Async
        public static async Task<CountryBusiness?> FindAsync(int id, ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            var country = await r.GetByIdAsync(id);
            if (country != null)
                return new CountryBusiness(country, enMode.Update, r);

            return null;
        }

        // 3. البحث باسم الدولة Async (دالة خاصة بالـ Country)
        public static async Task<CountryBusiness?> FindByNameAsync(string countryName, ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            var country = await r.GetByNameAsync(countryName);
            if (country != null)
                return new CountryBusiness(country, enMode.Update, r);

            return null;
        }

        // 4. حفظ البيانات (إضافة / تعديل) Async
        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    int newId = await _countryRepo.AddAsync(this.ToEntity());
                    if (newId > 0)
                    {
                        this.CountryId = newId;
                        this.Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return await _countryRepo.UpdateAsync(this.ToEntity());
            }
            return false;
        }

        // 5. حذف دولة Async
        public static async Task<bool> DeleteCountryAsync(int id, ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            return await r.DeleteAsync(id);
        }
    }
}
