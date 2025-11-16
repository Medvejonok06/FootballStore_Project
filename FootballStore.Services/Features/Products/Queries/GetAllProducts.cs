    using MediatR;
    using FootballStore.Services.DTOs;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    namespace FootballStore.Services.Features.Products.Queries
    {
        // Query - Запит на отримання даних (2.00 балів)
        public record GetAllProductsQuery : IRequest<IEnumerable<ProductDto>>;

        // QueryHandler - Обробник запиту
        public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
        {
            private readonly IProductService _productService; 

            public GetAllProductsQueryHandler(IProductService productService)
            {
                _productService = productService;
            }

            public async Task<IEnumerable<ProductDto>> Handle(
                GetAllProductsQuery request, 
                CancellationToken cancellationToken)
            {
                return await _productService.GetAllProductsAsync(cancellationToken);
            }
        }
    }