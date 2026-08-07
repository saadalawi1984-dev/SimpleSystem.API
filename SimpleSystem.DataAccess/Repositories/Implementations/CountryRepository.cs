using System.Data;
using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class CountryRepository : ICountryRepository
    {
        public List<Country> GetAll()
        {
            var list = new List<Country>();
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT CountryId, CountryName, CountryCode FROM Countries", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Country
                        {
                            CountryId = reader.GetInt32(0),
                            CountryName = reader.GetString(1),
                            CountryCode = reader.GetString(2)
                        });
                    }
                }
            }
            return list;
        }

        public Country? GetById(int countryId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT CountryId, CountryName, CountryCode FROM Countries WHERE CountryId = @CountryId", conn))
            {
                cmd.Parameters.AddWithValue("@CountryId", countryId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Country
                        {
                            CountryId = reader.GetInt32(0),
                            CountryName = reader.GetString(1),
                            CountryCode = reader.GetString(2)
                        };
                    }
                }
            }
            return null;
        }

        public int Add(Country country)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO Countries (CountryName, CountryCode) 
                VALUES (@CountryName, @CountryCode); 
                SELECT SCOPE_IDENTITY();", conn))
            {
                cmd.Parameters.AddWithValue("@CountryName", country.CountryName);
                cmd.Parameters.AddWithValue("@CountryCode", country.CountryCode);
                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public bool Update(Country country)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE Countries 
                SET CountryName = @CountryName, CountryCode = @CountryCode 
                WHERE CountryId = @CountryId", conn))
            {
                cmd.Parameters.AddWithValue("@CountryId", country.CountryId);
                cmd.Parameters.AddWithValue("@CountryName", country.CountryName);
                cmd.Parameters.AddWithValue("@CountryCode", country.CountryCode);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int countryId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("DELETE FROM Countries WHERE CountryId = @CountryId", conn))
            {
                cmd.Parameters.AddWithValue("@CountryId", countryId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}