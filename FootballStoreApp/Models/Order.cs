using System;
using System.Collections.Generic;

namespace FootballStoreApp.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; }
        
        // Зовнішній ключ (Customer ID), який використовується для зв'язку з Oracle
        public int CustomerId { get; set; }
        
        // Навігаційна властивість (ігнорується в ApplicationDbContext)
        public Customer Customer { get; set; }
        
        // Зв'язок 1:Багато
        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}