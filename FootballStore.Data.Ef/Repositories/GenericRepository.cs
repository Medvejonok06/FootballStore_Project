using FootballStore.Data.Ef.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FootballStore.Data.Ef.Repositories
{
    // Асинхронний Generic Repository (1.00 бал)
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> DbSet;

        public ApplicationDbContext Context => _context; // DbContext для Linq to Entities (JOIN)

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = _context.Set<TEntity>();
        }
        
        // Реалізація Eager/Explicit Loading (2.00 балів)
        public async Task<IEnumerable<TEntity>> GetAllAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (include != null)
            {
                // Застосування .Include() для Eager Loading
                query = include(query);
            }
            
            // Async Generic Repository з базовими методами
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        // --- Базовий CRUD ---
        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            // Використовуємо FindAsync, який шукає в кеші та БД
            return await DbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public async Task Update(TEntity entity)
        {
            DbSet.Update(entity);
            // SaveChangesAsync буде викликано зовнішнім сервісом (UoW pattern)
            await Task.CompletedTask;
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}