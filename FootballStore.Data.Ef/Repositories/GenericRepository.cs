using FootballStore.Data.Ef.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FootballStore.Data.Ef.Specifications; // <-- НОВИЙ USING

namespace FootballStore.Data.Ef.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> DbSet;

        public ApplicationDbContext Context => _context; 

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            DbSet = _context.Set<TEntity>();
        }
        
        // Метод для Eager/Explicit Loading (Вже реалізовано)
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
                query = include(query);
            }
            
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        // МЕТОД ДЛЯ ФІЛЬТРАЦІЇ ЧЕРЕЗ СПЕЦИФІКАЦІЮ (1.00 бал)
        public async Task<IEnumerable<TEntity>> GetBySpecificationAsync(
            ISpecification<TEntity> specification,
            CancellationToken cancellationToken = default)
        {
            // Застосовуємо критерій фільтрації
            IQueryable<TEntity> query = DbSet.Where(specification.Criteria);
            
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        // --- Базовий CRUD (Вже реалізовано) ---
        public async Task<TEntity?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public async Task Update(TEntity entity)
        {
            DbSet.Update(entity);
            await Task.CompletedTask;
        }

        public void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}