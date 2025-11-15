using Dapper;
using FootballStore.Data.Ado;
using Npgsql;
using System.Data;
using FootballStore.Data.Ado.Models;


namespace FootballStore.Data.Ado.Repositories
{
    // Dapper використовується для 2+ репозиторіїв (тут тільки Product)
    public class ProductRepository : IProductRepository
    {
        private readonly NpgsqlConnection _connection;
        private readonly NpgsqlTransaction _transaction;

        public ProductRepository(NpgsqlConnection connection, NpgsqlTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        // --- IGenericRepository Реалізація (Dapper) ---

        public async Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = "SELECT id, name, description, price, stockquantity FROM product WHERE id = @Id;";
            
            // Dapper спрощує виконання запитів і мапінг
            return await _connection.QueryFirstOrDefaultAsync<Product>(
                new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: cancellationToken));
        }

        public async Task<IEnumerable<Product>> GetAllAsync(CancellationToken cancellationToken)
        {
            const string sql = "SELECT * FROM product;";
            
            return await _connection.QueryAsync<Product>(
                new CommandDefinition(sql, transaction: _transaction, cancellationToken: cancellationToken));
        }

        public async Task<int> AddAsync(Product entity, CancellationToken cancellationToken)
        {
            const string sql = "INSERT INTO product (name, description, price, stockquantity) VALUES (@Name, @Description, @Price, @StockQuantity) RETURNING id;";
            
            return await _connection.ExecuteScalarAsync<int>(
                new CommandDefinition(sql, entity, _transaction, cancellationToken: cancellationToken));
        }

        public async Task<bool> UpdateAsync(Product entity, CancellationToken cancellationToken)
        {
            const string sql = "UPDATE product SET name = @Name, description = @Description, price = @Price, stockquantity = @StockQuantity WHERE id = @Id;";
            
            var rowsAffected = await _connection.ExecuteAsync(
                new CommandDefinition(sql, entity, _transaction, cancellationToken: cancellationToken));
            
            return rowsAffected > 0;
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
        {
            const string sql = "DELETE FROM product WHERE id = @Id;";
            
            var rowsAffected = await _connection.ExecuteAsync(
                new CommandDefinition(sql, new { Id = id }, _transaction, cancellationToken: cancellationToken));
            
            return rowsAffected > 0;
        }

        // --- IProductRepository Специфічний метод ---
        public async Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken)
        {
            const string sql = "SELECT * FROM product WHERE name = @Name;";
            
            return await _connection.QueryFirstOrDefaultAsync<Product>(
                new CommandDefinition(sql, new { Name = name }, _transaction, cancellationToken: cancellationToken));
        }
    }
}