using Microsoft.EntityFrameworkCore;
using FootballStoreApp.Models;

namespace FootballStoreApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Видалено DbSet<Customer>
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Кажемо EF Core ігнорувати навігаційну властивість Customer,
            // оскільки вона знаходиться в іншій БД.
            modelBuilder.Entity<Order>()
                .Ignore(o => o.Customer); 
            
            // Налаштування Order <-> OrderItem залишається
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            // Додаткові налаштування для Product
            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}