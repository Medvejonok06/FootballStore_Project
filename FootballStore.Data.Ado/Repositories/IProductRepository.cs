using System.Threading;
using FootballStore.Data.Ado.Models;

namespace FootballStore.Data.Ado{
    // Репозиторій для Product (Dapper)
    public interface IProductRepository : IGenericRepository<Product>
    {
        // Додаткові методи, які не входять у Generic Repository
        Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken);
    }
}