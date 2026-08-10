using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class PersonRepository : IPersonRepository
    {
        private const string BaseSelectQuery = "SELECT PersonId, FirstName, LastName, DateOfBirth, Phone, Email, CountryId FROM People";

        private Person MapReaderToPerson(SqlDataReader reader)
{
    int phoneOrdinal = reader.GetOrdinal("Phone");
    int emailOrdinal = reader.GetOrdinal("Email");

    return new Person
    {
        PersonId = reader.GetInt32(reader.GetOrdinal("PersonId")),
        FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
        LastName = reader.GetString(reader.GetOrdinal("LastName")),
        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
        Phone = reader.IsDBNull(phoneOrdinal) ? null : reader.GetString(phoneOrdinal),
        Email = reader.IsDBNull(emailOrdinal) ? null : reader.GetString(emailOrdinal),
        CountryId = reader.GetInt32(reader.GetOrdinal("CountryId"))
    };
}

        // 1. GetAllAsync
        public async Task<List<Person>> GetAllAsync()
        {
            var list = new List<Person>();
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(BaseSelectQuery, conn))
            {
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        list.Add(MapReaderToPerson((SqlDataReader)reader));
                    }
                }
            }
            return list;
        }

        // 2. GetByIdAsync
        public async Task<Person?> GetByIdAsync(int entityId)
        {
            string query = $"{BaseSelectQuery} WHERE PersonId = @PersonId";
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", entityId);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToPerson((SqlDataReader)reader);
                    }
                }
            }
            return null;
        }

        // 3. AddAsync
        public async Task<int> AddAsync(Person entity)
        {
            string query = @"
                INSERT INTO People (FirstName, LastName, DateOfBirth, Phone, Email, CountryId) 
                VALUES (@FirstName, @LastName, @DateOfBirth, @Phone, @Email, @CountryId); 
                SELECT SCOPE_IDENTITY();";

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", entity.FirstName ?? string.Empty);
                cmd.Parameters.AddWithValue("@LastName", entity.LastName ?? string.Empty);
                cmd.Parameters.AddWithValue("@DateOfBirth", entity.DateOfBirth);
                cmd.Parameters.AddWithValue("@Phone", (object?)entity.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)entity.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryId", entity.CountryId); // تم التعديل إلى CountryId

                await conn.OpenAsync();
                var result = await cmd.ExecuteScalarAsync();

                if (result != null && int.TryParse(result.ToString(), out int newId))
                {
                    entity.PersonId = newId;
                    return newId;
                }
            }
            return -1;
        }

        // 4. UpdateAsync
        public async Task<bool> UpdateAsync(Person entity)
        {
            string query = @"
                UPDATE People 
                SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, 
                    Phone = @Phone, Email = @Email, CountryId = @CountryId 
                WHERE PersonId = @PersonId";

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", entity.PersonId);
                cmd.Parameters.AddWithValue("@FirstName", entity.FirstName ?? string.Empty);
                cmd.Parameters.AddWithValue("@LastName", entity.LastName ?? string.Empty);
                cmd.Parameters.AddWithValue("@DateOfBirth", entity.DateOfBirth);
                cmd.Parameters.AddWithValue("@Phone", (object?)entity.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)entity.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryId", entity.CountryId); // تم التعديل إلى CountryId

                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        // 5. DeleteAsync
        public async Task<bool> DeleteAsync(int entityId)
        {
            string query = "DELETE FROM People WHERE PersonId = @PersonId";

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", entityId);
                await conn.OpenAsync();
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }

        // 6. GetByNationalNoAsync
        public async Task<Person?> GetByNationalNoAsync(string nationalNo)
        {
            string query = $"{BaseSelectQuery} WHERE NationalNo = @NationalNo";

            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@NationalNo", nationalNo);
                await conn.OpenAsync();
                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return MapReaderToPerson((SqlDataReader)reader);
                    }
                }
            }
            return null;
        }
    }
}
