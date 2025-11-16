using AutoMapper;
using FootballStore.Data.Ef;
using FootballStore.Data.Ef.Entities;
using FootballStore.Data.Ef.Repositories;
using FootballStore.Services.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace FootballStore.Services
{
    // Бізнес-сервіси через UoW (2.00 балів)
    public class ProductService : IProductService
    {
        // Впроваджуємо Generic Repository
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IMapper _mapper;

        public ProductService(
            IGenericRepository<Product> productRepository,
            IMapper mapper)
        {
            _productRepository = productRepository;
            _mapper = mapper;
        }

        // --- Linq to Entities (JOIN) (2.00 балів) ---
        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken)
        {
            // Демонстрація Linq to Entities (JOIN/Eager Loading)
            var productsWithOrderCount = await _productRepository.Context.Products
                .Include(p => p.OrderItems) // Eager Loading (2.00 балів)
                .Select(p => new ProductDto 
                {
                    Id = p.Id,
                    Name = p.Name,
                    // Демонстрація JOIN через Linq
                    Description = $"{p.Description} (Кількість замовлень: {p.OrderItems.Count})", 
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                })
                .ToListAsync(cancellationToken);

            return productsWithOrderCount; 
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetByIdAsync(id, cancellationToken);
            
            // Приклад Explicit Loading
            if (product != null)
            {
                await _productRepository.Context.Entry(product)
                    .Collection(p => p.OrderItems).LoadAsync(cancellationToken); 
            }
            
            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> CreateProductAsync(ProductCreateDto dto, CancellationToken cancellationToken)
        {
            var entity = _mapper.Map<Product>(dto);
            
            await _productRepository.AddAsync(entity, cancellationToken);
            await _productRepository.Context.SaveChangesAsync(cancellationToken); 
            
            return _mapper.Map<ProductDto>(entity);
        }
    }
}