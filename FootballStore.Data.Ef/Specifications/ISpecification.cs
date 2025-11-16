using System.Linq.Expressions;

namespace FootballStore.Data.Ef.Specifications
{
    // Інтерфейс для Specification pattern (1.00 бал)
    public interface ISpecification<T>
    {
        // Критерій фільтрації (WHERE clause)
        Expression<Func<T, bool>> Criteria { get; }
    }
}