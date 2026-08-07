using System.Data;
using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        public List<User> GetAll()
        {
            var list = new List<User>();
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT UserId, PersonId, Username, PasswordHash, IsActive, CreatedDate FROM Users", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new User
                        {
                            UserId = reader.GetInt32(0),
                            PersonId = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            PasswordHash = reader.GetString(3),
                            IsActive = reader.GetBoolean(4),
                            CreatedDate = reader.GetDateTime(5)
                        });
                    }
                }
            }
            return list;
        }

        public User? GetById(int userId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT UserId, PersonId, Username, PasswordHash, IsActive, CreatedDate FROM Users WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(0),
                            PersonId = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            PasswordHash = reader.GetString(3),
                            IsActive = reader.GetBoolean(4),
                            CreatedDate = reader.GetDateTime(5)
                        };
                    }
                }
            }
            return null;
        }

        public User? GetByPersonId(int personId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT UserId, PersonId, Username, PasswordHash, IsActive, CreatedDate FROM Users WHERE PersonId = @PersonId", conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", personId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new User
                        {
                            UserId = reader.GetInt32(0),
                            PersonId = reader.GetInt32(1),
                            Username = reader.GetString(2),
                            PasswordHash = reader.GetString(3),
                            IsActive = reader.GetBoolean(4),
                            CreatedDate = reader.GetDateTime(5)
                        };
                    }
                }
            }
            return null;
        }

        public int Add(User user)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Users (PersonId, Username, PasswordHash, IsActive, CreatedDate) 
                VALUES (@PersonId, @Username, @PasswordHash, @IsActive, GETDATE()); 
                SELECT SCOPE_IDENTITY();", conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", user.PersonId);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public bool Update(User user)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE Users 
                SET Username = @Username, PasswordHash = @PasswordHash, IsActive = @IsActive 
                WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", user.UserId);
                cmd.Parameters.AddWithValue("@Username", user.Username);
                cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                cmd.Parameters.AddWithValue("@IsActive", user.IsActive);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int userId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("DELETE FROM Users WHERE UserId = @UserId", conn))
            {
                cmd.Parameters.AddWithValue("@UserId", userId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}