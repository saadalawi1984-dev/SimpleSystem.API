using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private const string BaseSelectQuery = @"
            SELECT UserId, PersonId, Username, PasswordHash, IsActive, CreatedDate 
            FROM Users";

        private User MapReaderToUser(SqlDataReader reader)
        {
            return new User
            {
                UserId = Convert.ToInt32(reader["UserId"]),
                PersonId = Convert.ToInt32(reader["PersonId"]),
                Username = reader["Username"] as string ?? string.Empty,
                PasswordHash = reader["PasswordHash"] as string ?? string.Empty,
                IsActive = reader["IsActive"] != DBNull.Value && Convert.ToBoolean(reader["IsActive"]),
                CreatedDate = reader["CreatedDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(reader["CreatedDate"])
            };
        }

        private async Task<User?> GetSingleUserAsync(string query, params SqlParameter[] parameters)
        {
            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                if (parameters != null)
                {
                    command.Parameters.AddRange(parameters);
                }

                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToUser(reader);
                    }
                }
            }
            return null;
        }

        

        public async Task<List<User>> GetAllAsync()
        {
            var usersList = new List<User>();

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(BaseSelectQuery, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        usersList.Add(MapReaderToUser(reader));
                    }
                }
            }

            return usersList;
        }

        public async Task<User?> GetByIdAsync(int entityId)
        {
            string query = $"{BaseSelectQuery} WHERE UserId = @UserId";
            return await GetSingleUserAsync(query, new SqlParameter("@UserId", entityId));
        }

        public async Task<int> AddAsync(User entity)
        {
            string query = @"
                INSERT INTO Users (PersonId, Username, PasswordHash, IsActive, CreatedDate)
                VALUES (@PersonId, @Username, @PasswordHash, @IsActive, @CreatedDate);
                SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PersonId", entity.PersonId);
                command.Parameters.AddWithValue("@Username", entity.Username);
                command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                command.Parameters.AddWithValue("@IsActive", (object?)entity.IsActive ?? DBNull.Value);
                command.Parameters.AddWithValue("@CreatedDate", (object?)entity.CreatedDate ?? DateTime.Now);

                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    entity.UserId = newId;
                    return newId;
                }
            }

            return -1;
        }

        public async Task<bool> UpdateAsync(User entity)
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
                command.Parameters.AddWithValue("@UserId", entity.UserId);
                command.Parameters.AddWithValue("@PersonId", entity.PersonId);
                command.Parameters.AddWithValue("@Username", entity.Username);
                command.Parameters.AddWithValue("@PasswordHash", entity.PasswordHash);
                command.Parameters.AddWithValue("@IsActive", (object?)entity.IsActive ?? DBNull.Value);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> DeleteAsync(int entityId)
        {
            string query = "DELETE FROM Users WHERE UserId = @UserId";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserId", entityId);

                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        

        public async Task<User?> GetByPersonIdAsync(int personId)
        {
            string query = $"{BaseSelectQuery} WHERE PersonId = @PersonId";
            return await GetSingleUserAsync(query, new SqlParameter("@PersonId", personId));
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            string query = $"{BaseSelectQuery} WHERE Username = @Username";
            return await GetSingleUserAsync(query, new SqlParameter("@Username", username));
        }
    }
}
