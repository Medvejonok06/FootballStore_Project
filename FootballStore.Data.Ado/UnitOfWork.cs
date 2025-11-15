using Npgsql;
using System.Data;
using FootballStore.Data.Ado.Models;
using FootballStore.Data.Ado.Repositories;


namespace FootballStore.Data.Ado
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly NpgsqlConnection _connection;
        private NpgsqlTransaction? _transaction;

        // Інтерфейси репозиторіїв
        public ICustomerRepository Customers { get; }
        public IProductRepository Products { get; }

        public UnitOfWork(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
            _connection.Open();
            
            // Починаємо транзакцію одразу
            _transaction = _connection.BeginTransaction();

            // Ініціалізуємо репозиторії, передаючи їм підключення та транзакцію
            // Це дозволяє всім репозиторіям працювати в рамках однієї транзакції
            Customers = new CustomerRepository(_connection, _transaction);
            Products = new ProductRepository(_connection, _transaction);
        }

        // --- Реалізація Unit of Work з Транзакціями ---

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                _transaction.Dispose();
                _transaction = null;
            }
        }

        // --- Реалізація IDisposable ---

        public void Dispose()
        {
            // Якщо транзакція досі активна, вона має бути відкатана перед закриттям підключення
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
            }
            if (_connection != null && _connection.State != ConnectionState.Closed)
            {
                _connection.Close();
                _connection.Dispose();
            }
            GC.SuppressFinalize(this);
        }
    }
}