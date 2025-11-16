using Microsoft.EntityFrameworkCore;
using FootballStore.Data.Ef.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FootballStore.Data.Ef
{
    public class ApplicationDbContext : DbContext
    {
        // DbSet для всіх сутностей
        public DbSet<Product> Products { get; set; } 
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // --- Fluent API конфігурація через IEntityTypeConfiguration (1.00 балів) ---
            
            modelBuilder.ApplyConfiguration(new ProductConfiguration());

            // Налаштування Customer (Унікальний індекс для Email)
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // Налаштування Order Item (Композитний ключ)
            modelBuilder.Entity<OrderItem>()
                .HasKey(oi => new { oi.OrderId, oi.ProductId });
            
            // Налаштування зв'язків (Order <-> Customer)
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Seeding (Наповнення бази даних - 1.00 балів) ---
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Футболка Збірна України (EF)", Description = "Домашня форма 2024", Price = 1200.00m, StockQuantity = 50 },
                new Product { Id = 2, Name = "М'яч Adidas Roteiro (EF)", Description = "Офіційний м'яч Євро-2004", Price = 3500.50m, StockQuantity = 5 },
                new Product { Id = 3, Name = "Шарф ФК Динамо Київ (EF)", Description = "Вболівальницький шарф", Price = 450.00m, StockQuantity = 100 }
            );

            base.OnModelCreating(modelBuilder);
        }
    }

    // Fluent API конфігурація через IEntityTypeConfiguration<T> (1.00 балів)
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18, 2)");
        }
    }
}