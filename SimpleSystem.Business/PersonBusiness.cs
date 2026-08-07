using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;

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

        public static List<Person> GetAllPeople(IPersonRepository? repo = null)
        {
            var r = repo ?? new PersonRepository();
            return r.GetAll();
        }

        public static PersonBusiness? Find(int id, IPersonRepository? repo = null)
        {
            var r = repo ?? new PersonRepository();
            var person = r.GetById(id);
            if (person != null)
                return new PersonBusiness(person, enMode.Update, r);

            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    int newId = _personRepo.Add(this.ToEntity());
                    if (newId > 0)
                    {
                        this.PersonId = newId;
                        this.Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _personRepo.Update(this.ToEntity());
            }
            return false;
        }

        public static bool DeletePerson(int id, IPersonRepository? repo = null)
        {
            var r = repo ?? new PersonRepository();
            return r.Delete(id);
        }
    }
}