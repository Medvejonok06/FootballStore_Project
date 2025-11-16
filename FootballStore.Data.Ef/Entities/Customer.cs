using System.ComponentModel.DataAnnotations;
using System.Collections.Generic; // Додано для ICollection

namespace FootballStore.Data.Ef.Entities
{
    // Entity з ключами та обмеженнями (1.00 балів)
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        
        // Навігаційна властивість для зв'язку з Order
        public ICollection<Order> Orders { get; set; } = new List<Order>();
    }
}