using System.ComponentModel.DataAnnotations;

namespace FootballStore.Data.Ef.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        
        // Зовнішній ключ
        public int CustomerId { get; set; } 
        
        // Навігаційні властивості
        public Customer Customer { get; set; } = default!;
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}