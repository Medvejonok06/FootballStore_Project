using FluentValidation;
using FootballStore.Services.DTOs;
using Microsoft.EntityFrameworkCore;

namespace FootballStore.Services.Validation
{
    // Валідація (FluentValidation) (2.00 балів)
    public class ProductCreateDtoValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateDtoValidator()
        {
            // Перевірка назви: не пуста і не довша за 150 символів
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Назва продукту є обов'язковою.")
                .MaximumLength(150).WithMessage("Назва не може перевищувати 150 символів.");

            // Перевірка ціни: має бути більше нуля
            RuleFor(p => p.Price)
                .GreaterThan(0).WithMessage("Ціна має бути більше нуля.");

            // Перевірка кількості на складі: не може бути від'ємною
            RuleFor(p => p.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Кількість на складі не може бути від'ємною.");
        }
    }
}