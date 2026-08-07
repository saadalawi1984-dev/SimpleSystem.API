using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;
using System.Diagnostics.Metrics;

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

        public static List<Country> GetAllCountries(ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            return r.GetAll();
        }

        public static CountryBusiness? Find(int id, ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            var country = r.GetById(id);
            if (country != null)
                return new CountryBusiness(country, enMode.Update, r);

            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    int newId = _countryRepo.Add(this.ToEntity());
                    if (newId > 0)
                    {
                        this.CountryId = newId;
                        this.Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _countryRepo.Update(this.ToEntity());
            }
            return false;
        }

        public static bool DeleteCountry(int id, ICountryRepository? repo = null)
        {
            var r = repo ?? new CountryRepository();
            return r.Delete(id);
        }
    }
}