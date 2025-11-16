using FootballStore.Services.DTOs;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FootballStore.Services
{
    // Бізнес-сервіси через UoW (2.00 балів)
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken);
        Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken);
        Task<ProductDto> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken);
    }
}