using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        private const string BaseSelectQuery = "SELECT CountryId, CountryName FROM Countries";

        private Country MapReaderToCountry(SqlDataReader reader)
        {
            return new Country
            {
                CountryId = Convert.ToInt32(reader["CountryId"]),
                CountryName = reader["CountryName"] as string ?? string.Empty
            };
        }

        // 1. GetAllAsync
        public async Task<List<Country>> GetAllAsync()
        {
            var list = new List<Country>();
            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(BaseSelectQuery, connection))
            {
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(MapReaderToCountry(reader));
                    }
                }
            }
            return list;
        }

        // 2. GetByIdAsync
        public async Task<Country?> GetByIdAsync(int entityId)
        {
            string query = $"{BaseSelectQuery} WHERE CountryId = @CountryId";
            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryId", entityId);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return MapReaderToCountry(reader);
                }
            }
            return null;
        }

        // 3. AddAsync
        public async Task<int> AddAsync(Country entity)
        {
            string query = @"
                INSERT INTO Countries (CountryName) VALUES (@CountryName);
                SELECT SCOPE_IDENTITY();";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryName", entity.CountryName);
                await connection.OpenAsync();
                var result = await command.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    entity.CountryId = newId;
                    return newId;
                }
            }
            return -1;
        }

        // 4. UpdateAsync
        public async Task<bool> UpdateAsync(Country entity)
        {
            string query = "UPDATE Countries SET CountryName = @CountryName WHERE CountryId = @CountryId";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryId", entity.CountryId);
                command.Parameters.AddWithValue("@CountryName", entity.CountryName);
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        // 5. DeleteAsync
        public async Task<bool> DeleteAsync(int entityId)
        {
            string query = "DELETE FROM Countries WHERE CountryId = @CountryId";

            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryId", entityId);
                await connection.OpenAsync();
                int rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        // 6. GetByNameAsync (الدالة الخاصة)
        public async Task<Country?> GetByNameAsync(string countryName)
        {
            string query = $"{BaseSelectQuery} WHERE CountryName = @CountryName";
            using (var connection = new SqlConnection(DataAccessSettings.ConnectionString))
            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@CountryName", countryName);
                await connection.OpenAsync();
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                        return MapReaderToCountry(reader);
                }
            }
            return null;
        }
    }
}
