using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Interfaces;
using System;
using System.Collections.Generic;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private const string BaseSelectQuery = @"
            SELECT UserId, PersonId, Username, PasswordHash, IsActive, CreatedDate 
            FROM Users";

        // دالة تحويل البيانات وآمنة من الـ Null
        private User MapReaderToUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                PersonId = Convert.ToInt32(reader["PersonId"]),
                Username = reader["Username"] as string ?? string.Empty,
                PasswordHash = reader["PasswordHash"] as string ?? string.Empty,

                // حل أخطاء التحويل الضمني (Implicit conversion of bool?)
                // نستخدم GetValueOrDefault لإعطاء قيمة افتراضية في حال كانت null
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),

                CreatedDate = reader["CreatedDate"] == DBNull.Value
                    ? DateTime.Now
                    : Convert.ToDateTime(reader["CreatedDate"])
            };
        }

        private User? GetSingleUser(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return null;
        }

       

        public List<User> GetAll()
        {
            var usersList = new List<User>();

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(BaseSelectQuery, connection))
            {
                connection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        usersList.Add(MapReaderToUser(reader));
                    }
                }
            }

            return usersList;
        }

        public User? GetById(int userId)
        {
            string query = $"{BaseSelectQuery} WHERE UserId = @UserId";
            return GetSingleUser(query, new SqlParameter("@UserId", userId));
        }

        public User? GetByUsername(string username)
        {
            string query = $"{BaseSelectQuery} WHERE Username = @Username";
            return GetSingleUser(query, new SqlParameter("@Username", username));
        }

        
        public User? GetByPersonId(int personId)
        {
            string query = $"{BaseSelectQuery} WHERE PersonId = @PersonId";
            return GetSingleUser(query, new SqlParameter("@PersonId", personId));
        }

        
        public int Add(User user)
        {
            string query = @"
        INSERT INTO Users (PersonId, Username, PasswordHash, IsActive, CreatedDate)
        VALUES (@PersonId, @Username, @PasswordHash, @IsActive, GETDATE());
        SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PersonId", user.PersonId);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);

                // التعامل مع Null بحذر للـ IsActive
                command.Parameters.AddWithValue("@IsActive", (object?)user.IsActive ?? DBNull.Value);

                connection.Open();
                var result = command.ExecuteScalar();

                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    user.UserId = newId;
                    return newId; // إرجاع الرقم التعريفي الجديد (ID)
                }
            }

            return -1; // إرجاع -1 في حال فشلت الإضافة
        }

        // 3. حل خطأ: Update(User)
        public bool Update(User user)
        {
            string query = @"
                UPDATE Users 
                SET PersonId = @PersonId,
                    Username = @Username,
                    PasswordHash = @PasswordHash,
                    IsActive = @IsActive
                WHERE UserId = @UserId";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", user.UserId);
                command.Parameters.AddWithValue("@PersonId", user.PersonId);
                command.Parameters.AddWithValue("@Username", user.Username);
                command.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
                command.Parameters.AddWithValue("@IsActive", user.IsActive);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // 4. حل خطأ: Delete(int)
        public bool Delete(int userId)
        {
            string query = "DELETE FROM Users WHERE UserId = @UserId";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
    }
}
