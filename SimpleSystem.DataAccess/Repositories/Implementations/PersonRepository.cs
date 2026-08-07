using System.Data;
using Microsoft.Data.SqlClient;
using SimpleSystem.DataAccess.Data;
using SimpleSystem.DataAccess.Entities;
using SimpleSystem.DataAccess.Repositories.Interfaces;

namespace SimpleSystem.DataAccess.Repositories.Implementations
{
    public class PersonRepository : IPersonRepository
    {
        public List<Person> GetAll()
        {
            var list = new List<Person>();
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT PersonId, FirstName, LastName, DateOfBirth, Phone, Email, CountryId FROM People", conn))
            {
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        list.Add(new Person
                        {
                            PersonId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            DateOfBirth = reader.GetDateTime(3),
                            Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CountryId = reader.GetInt32(6)
                        });
                    }
                }
            }
            return list;
        }

        public Person? GetById(int personId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("SELECT PersonId, FirstName, LastName, DateOfBirth, Phone, Email, CountryId FROM People WHERE PersonId = @PersonId", conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", personId);
                conn.Open();
                using (var reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return new Person
                        {
                            PersonId = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            DateOfBirth = reader.GetDateTime(3),
                            Phone = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Email = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CountryId = reader.GetInt32(6)
                        };
                    }
                }
            }
            return null;
        }

        public int Add(Person person)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(@"
                INSERT INTO People (FirstName, LastName, DateOfBirth, Phone, Email, CountryId) 
                VALUES (@FirstName, @LastName, @DateOfBirth, @Phone, @Email, @CountryId); 
                SELECT SCOPE_IDENTITY();", conn))
            {
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
                cmd.Parameters.AddWithValue("@Phone", (object?)person.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)person.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryId", person.CountryId);

                conn.Open();
                var result = cmd.ExecuteScalar();
                return Convert.ToInt32(result);
            }
        }

        public bool Update(Person person)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand(@"
                UPDATE People 
                SET FirstName = @FirstName, LastName = @LastName, DateOfBirth = @DateOfBirth, 
                    Phone = @Phone, Email = @Email, CountryId = @CountryId 
                WHERE PersonId = @PersonId", conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", person.PersonId);
                cmd.Parameters.AddWithValue("@FirstName", person.FirstName);
                cmd.Parameters.AddWithValue("@LastName", person.LastName);
                cmd.Parameters.AddWithValue("@DateOfBirth", person.DateOfBirth);
                cmd.Parameters.AddWithValue("@Phone", (object?)person.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@Email", (object?)person.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@CountryId", person.CountryId);

                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }

        public bool Delete(int personId)
        {
            using (var conn = SqlConnectionFactory.CreateConnection())
            using (var cmd = new SqlCommand("DELETE FROM People WHERE PersonId = @PersonId", conn))
            {
                cmd.Parameters.AddWithValue("@PersonId", personId);
                conn.Open();
                return cmd.ExecuteNonQuery() > 0;
            }
        }
    }
}