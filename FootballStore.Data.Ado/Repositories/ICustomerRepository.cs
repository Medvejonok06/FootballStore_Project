using System.Threading;
using FootballStore.Data.Ado.Models;

namespace FootballStore.Data.Ado
{
    // Репозиторій для Customer (Чистий ADO.NET)
    public interface ICustomerRepository
    {
        // Метод для демонстрації параметризованих запитів
        Task<Customer?> GetByIdAsync(int id, CancellationToken cancellationToken); 
        
        // Метод для створення нового клієнта
        Task<int> AddAsync(Customer customer, CancellationToken cancellationToken); 
    }
}