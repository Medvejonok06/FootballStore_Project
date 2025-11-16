using System.ComponentModel.DataAnnotations;

namespace FootballStore.Services.DTOs
{
    // DTO для створення продукту (Використовується у POST запитах)
    public class ProductCreateDto
    {
        // required гарантує, що поле не може бути null
        public required string Name { get; set; }
        public required string Description { get; set; }
        
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
    }
}