using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        public DateTime CreatedDate { get; set; } = DateTime.Now;

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
            this.IsActive = entity.IsActive ?? true;
            this.CreatedDate = entity.CreatedDate ?? DateTime.Now;
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

        public static async Task<List<User>> GetAllUsersAsync(IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            return await r.GetAllAsync();
        }

        public static async Task<UserBusiness?> FindAsync(int id, IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            var user = await r.GetByIdAsync(id);
            if (user != null)
                return new UserBusiness(user, enMode.Update, r);

            return null;
        }

        public static async Task<UserBusiness?> FindByPersonIdAsync(int personId, IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            var user = await r.GetByPersonIdAsync(personId);
            if (user != null)
                return new UserBusiness(user, enMode.Update, r);

            return null;
        }

        public async Task<bool> SaveAsync()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    int newId = await _userRepo.AddAsync(this.ToEntity());
                    if (newId > 0)
                    {
                        this.UserId = newId;
                        this.Mode = enMode.Update;
                        return true;
                    }
                    return false;

                case enMode.Update:
                    return await _userRepo.UpdateAsync(this.ToEntity());
            }
            return false;
        }

        public static async Task<bool> DeleteUserAsync(int id, IUserRepository? repo = null)
        {
            var r = repo ?? new UserRepository();
            return await r.DeleteAsync(id);
        }
    }
}
