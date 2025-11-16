using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace FootballStore.Data.Ef
{
    // Клас-фабрика, необхідний EF Core Tools для створення контексту в режимі дизайну
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // У режимі дизайну використовуємо жорстко заданий рядок підключення
            const string designConnectionString = "Host=localhost;Database=football_db_ef;Username=postgres;Password=12345";
            
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(designConnectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}