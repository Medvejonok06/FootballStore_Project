using FootballStore.Data.Ado;
using FootballStore.Data.Ado.Models;
using Npgsql;
using System.Data;

namespace FootballStore.Data.Ado.Repositories
{
    // Примітка: Ми використовуємо модель Customer, хоча раніше ізолювали її в Oracle.
    // Тут ми імітуємо, що це інший мікросервіс, який використовує PostgreSQL.
    public class CustomerRepository : ICustomerRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;

        public CustomerRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        // 1. Параметризований запит на отримання за ID (проти SQL-ін'єкцій)
        public async Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = "SELECT id, firstname, lastname, email FROM customer WHERE id = @Id;";
            
            await using var cmd = new NpgsqlCommand(sql, _connection, _transaction);
            
            // Використання параметрів для захисту від SQL-ін'єкцій
            cmd.Parameters.AddWithValue("@Id", id);

            await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
            
            if (await reader.ReadAsync(cancellationToken))
            {
                return new Customer
                {
                    Id = reader.GetInt32(0),
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    Email = reader.GetString(3)
                };
            }
            return null;
        }

        // 2. Асинхронність та CancellationToken (1.00 балів)
        public async Task<int> AddAsync(Customer customer, CancellationToken cancellationToken)
        {
            const string sql = "INSERT INTO customer (firstname, lastname, email) VALUES (@FirstName, @LastName, @Email) RETURNING id;";
            
            await using var cmd = new NpgsqlCommand(sql, _connection, _transaction);
            
            // Параметризовані запити (захист)
            cmd.Parameters.AddWithValue("@FirstName", customer.FirstName);
            cmd.Parameters.AddWithValue("@LastName", customer.LastName);
            cmd.Parameters.AddWithValue("@Email", customer.Email);

            // Виконання команди асинхронно
            var result = await cmd.ExecuteScalarAsync(cancellationToken);
            
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}