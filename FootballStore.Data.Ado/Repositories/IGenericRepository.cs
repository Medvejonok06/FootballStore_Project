using System.Collections.Generic;

namespace FootballStore.Data.Ado
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken);
        Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
        Task<int> AddAsync(T entity, CancellationToken cancellationToken);
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
    }
}