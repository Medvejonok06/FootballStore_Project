namespace FootballStore.Data.Ef.Entities
{
    public class OrderItem
    {
        // Не використовуємо Id, оскільки буде композитний ключ
        
        public int OrderId { get; set; }
        public Order Order { get; set; } = default!;
        
        public int ProductId { get; set; }
        public Product Product { get; set; } = default!;
        
        public int Quantity { get; set; }
        public decimal PricePerUnit { get; set; } 
    }
}