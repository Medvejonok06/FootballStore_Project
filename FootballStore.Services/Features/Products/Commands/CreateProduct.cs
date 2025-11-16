using MediatR;
using FootballStore.Services.DTOs;
using FluentValidation;
using System.Threading;
using System.Threading.Tasks;

namespace FootballStore.Services.Features.Products.Commands
{
    // Command - Запит на зміну стану (2.00 балів)
    public record CreateProductCommand : IRequest<ProductDto>
    {
        public required ProductCreateDto Dto { get; init; }
    }

    // CommandHandler
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
    {
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<ProductDto> Handle(
            CreateProductCommand request, 
            CancellationToken cancellationToken)
        {
            return await _productService.CreateProductAsync(request.Dto, cancellationToken);
        }
    }

    // Валідатор для Command
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator(IValidator<ProductCreateDto> dtoValidator)
        {
            RuleFor(x => x.Dto).SetValidator(dtoValidator);
        }
    }
}