using FootballStore.Data.Ef;
using FootballStore.Data.Ef.Repositories;
using FootballStore.Services;
using FootballStore.Services.Mapping;
using FootballStore.Services.Validation;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi;
using Serilog;
using Serilog.Enrichers;
using Serilog.Events;
using System.Reflection;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                     // Мінімальний рівень логування
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning) // менше шуму від Microsoft
    .Enrich.WithMachineName()                       // додає ім'я машини
    .Enrich.WithMachineName()                       // додає ім'я машини
    .Enrich.WithEnvironmentName()                  // додає середовище (Development/Production)
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}") // консоль
    .WriteTo.File(
        path: "logs/footballstore-.log",          // файл для логів
        rollingInterval: RollingInterval.Day,     // щоденна ротація
        retainedFileCountLimit: 7,                // зберігати 7 днів
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss} {Level:u3}] {Message:lj} {Properties}{NewLine}{Exception}"
    )
    .CreateLogger();

try
{
    Log.Information("Запуск Football Store API...");

    var builder = WebApplication.CreateBuilder(args);

    // Підключення Serilog до ASP.NET Core
    builder.Host.UseSerilog();

    // --- Конфігурація EF Core та PostgreSQL ---
    var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ??
                           "Host=localhost;Database=football_db_ef;Username=postgres;Password=12345";

    // --- Реєстрація сервісів ---
    builder.Services.AddControllers();

    // Swagger/OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Football Store API (EF Core)", Version = "v1" });
    });

    // EF Core DbContext
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(connectionString));

    // AutoMapper
    builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

    // FluentValidation
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);

    // Repositories та Services
    builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
    builder.Services.AddScoped<IProductService, ProductService>();

    var app = builder.Build();

    // --- Автоматичне застосування міграцій ---
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();
    }

    // --- Middleware ---
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    else
    {
        app.UseExceptionHandler("/error");
    }

    app.UseAuthorization();
    app.MapControllers();

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Програма завершилася з фатальною помилкою");
}
finally
{
    Log.CloseAndFlush();
}
