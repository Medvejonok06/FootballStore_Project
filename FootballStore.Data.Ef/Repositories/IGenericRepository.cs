using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using FootballStore.Data.Ef.Entities;
using FootballStore.Data.Ef.Specifications; // <-- НОВИЙ USING

namespace FootballStore.Data.Ef.Repositories
{
    // Async Generic Repository з базовими методами (1.00 балів)
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        // Метод для Eager/Explicit Loading
        Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            CancellationToken cancellationToken = default);

        // МЕТОД ДЛЯ ФІЛЬТРАЦІЇ ЧЕРЕЗ СПЕЦИФІКАЦІЮ (1.00 бал)
        Task<IEnumerable<TEntity>> GetBySpecificationAsync(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default);
        
        Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
        
        Task AddAsync(TEntity entity, CancellationToken cancellationToken = default);
        Task Update(TEntity entity);
        void Delete(TEntity entity); 

        // DbContext для Linq to Entities (JOIN)
        ApplicationDbContext Context { get; }
    }
}