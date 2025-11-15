using System; // Додаємо, оскільки IDisposable знаходиться тут
using System.Threading.Tasks;

namespace FootballStore.Data.Ado
{
    public interface IUnitOfWork : IDisposable
    {
        // Репозиторій на чистому ADO.NET
        ICustomerRepository Customers { get; }
        
        // Репозиторій на ADO.NET + Dapper
        IProductRepository Products { get; }
        
        // Метод для фіксації змін у транзакції (0.50 балів)
        Task CommitAsync();
        
        // Метод для відкату транзакції
        Task RollbackAsync();
    }
}