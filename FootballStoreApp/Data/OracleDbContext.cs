using Microsoft.EntityFrameworkCore;
using FootballStoreApp.Models;

namespace FootballStoreApp.Data
{
    public class OracleDbContext : DbContext
    {
        // Містить лише одну модель - Customer
        public DbSet<Customer> Customers { get; set; }

        public OracleDbContext(DbContextOptions<OracleDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Назва таблиці в Oracle має бути у верхньому регістрі
            modelBuilder.Entity<Customer>().ToTable("CUSTOMERS");
            base.OnModelCreating(modelBuilder);
        }
    }
}