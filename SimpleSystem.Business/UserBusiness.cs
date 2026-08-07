using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.Business
{
    public class UserBusiness
    {
        private readonly IUserRepository _userRepo;

        public enum enMode { AddNew = 0, Update = 1 }
        public enMode Mode { get; set; } = enMode.AddNew;

        public int UserId { get; set; }
        public int PersonId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; }

        public UserBusiness(IUserRepository? userRepo = null)
        {
            _userRepo = userRepo ?? new UserRepository();
        }

        private UserBusiness(User entity, enMode mode, IUserRepository? userRepo = null)
        {
            _userRepo = userRepo ?? new UserRepository();
            this.UserId = entity.UserId;
            this.PersonId = entity.PersonId;
            this.Username = entity.Username;
            this.PasswordHash = entity.PasswordHash;
            this.IsActive = entity.IsActive;
            this.CreatedDate = entity.CreatedDate;
            this.Mode = mode;
        }

        public User ToEntity()
        {
            return new User
            {
                UserId = this.UserId,
                PersonId = this.PersonId,
                Username = this.Username,
                PasswordHash = this.PasswordHash,
                IsActive = this.IsActive,
                CreatedDate = this.CreatedDate
            };
        }

        public static List<User> GetAllUsers(IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            return r.GetAll();
        }

        public static UserBusiness? Find(int id, IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            var user = r.GetById(id);
            if (user != null)
                return new UserBusiness(user, enMode.Update, r);

            return null;
        }

        public static UserBusiness? FindByPersonId(int personId, IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            var user = r.GetByPersonId(personId);
            if (user != null)
                return new UserBusiness(user, enMode.Update, r);

            return null;
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    int newId = _userRepo.Add(this.ToEntity());
                    if (newId > 0)
                    {
                        this.UserId = newId;
                        this.Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return _userRepo.Update(this.ToEntity());
            }
            return false;
        }

        public static bool DeleteUser(int id, IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            return r.Delete(id);
        }
    }
}