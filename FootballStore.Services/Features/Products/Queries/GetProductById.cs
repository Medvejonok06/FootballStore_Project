using MediatR;
using FootballStore.Services.DTOs;

namespace FootballStore.Services.Features.Products.Queries
{
    // Query для отримання одного продукту
    public record GetProductByIdQuery : IRequest<ProductDto?>
    {
        public int Id { get; init; }
    }
    
    // ПРИМІТКА: Handler для цього запиту потрібно створити, але зараз це не є блокером.
}