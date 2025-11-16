using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace FootballStore.Data.Ef.Entities
{
    // Entity з ключами та обмеженнями (1.00 балів)
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } 
        
        public required string Name { get; set; } 
        public required string Description { get; set; } 
        
        [Column(TypeName = "decimal(18, 2)")] // Визначення типу для точності
        public decimal Price { get; set; } 
        
        public int StockQuantity { get; set; } 

        // Навігаційна властивість для зв'язку з OrderItem
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}