using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SimpleSystem.Business
{
    public class PersonBusiness
    {
        private readonly IPersonRepository _personRepo;

        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int PersonId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int CountryId { get; set; }

        // Constructor للتنفيذ والـ Dependency Injection
        public PersonBusiness(IPersonRepository? personRepo = null)
        {
            _personRepo = personRepo ?? new PersonRepository();
        }

        private PersonBusiness(Person entity, enMode mode, IPersonRepository? personRepo = null)
        {
            _personRepo = personRepo ?? new PersonRepository();
            this.PersonId = entity.PersonId;
            this.FirstName = entity.FirstName;
            this.LastName = entity.LastName;
            this.DateOfBirth = entity.DateOfBirth;
            this.Phone = entity.Phone;
            this.Email = entity.Email;
            this.CountryId = entity.CountryId;
            this.Mode = mode;
        }

        public Person ToEntity()
        {
            return new Person
            {
                PersonId = this.PersonId,
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                Phone = this.Phone,
                Email = this.Email,
                CountryId = this.CountryId
            };
        }

        // 1. جلب كافة الأشخاص Async
        public static async Task<List<Person>> GetAllPeopleAsync(IPersonRepository? repo = null)
        {
            var r = repo ?? new PersonRepository();
            return await r.GetAllAsync();
        }

        // 2. البحث برقم الشخص Async
        public static async Task<PersonBusiness?> FindAsync(int id, IPersonRepository? repo = null)
        {
            var r = repo ?? new PersonRepository();
            var person = await r.GetByIdAsync(id);
            if (person != null)
                return new PersonBusiness(person, enMode.Update, r);

            return null;
        }

        // 3. حفظ البيانات (إضافة / تعديل) Async
        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    int newId = await _personRepo.AddAsync(this.ToEntity());
                    if (newId > 0)
                    {
                        this.PersonId = newId;
                        this.Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return await _personRepo.UpdateAsync(this.ToEntity());
            }
            return false;
        }

        // 4. حذف شخص Async
        public static async Task<bool> DeletePersonAsync(int id, IPersonRepository? repo = null)
        {
            var r = repo ?? new PersonRepository();
            return await r.DeleteAsync(id);
        }
    }
}
