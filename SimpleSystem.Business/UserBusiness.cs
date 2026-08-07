using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Implementations;
using SimpleSystem.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;

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

        // أنواع البيانات في طبقة Business
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

            // 1. حل خطأ التحويل للـ bool?: إعطاء true كقيمة افتراضية إذا كانت Null
            this.IsActive = entity.IsActive ?? true;

            // 2. حل خطأ التحويل للـ DateTime?: إعطاء DateTime.Now كقيمة افتراضية إذا كانت Null
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
