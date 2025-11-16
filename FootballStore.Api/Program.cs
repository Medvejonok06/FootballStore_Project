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
using FootballStore.Api.Middleware;
using MediatR; // <-- НОВИЙ USING для CQRS/MediatR

// 1. НАЛАШТУВАННЯ SERILOG (Логування - 1.00 бал)
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder().AddJsonFile("appsettings.json").Build())
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Інтеграція Serilog з хостом
builder.Host.UseSerilog(); 

// --- Конфігурація EF Core та PostgreSQL ---
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ?? 
                       "Host=localhost;Database=football_db_ef;Username=postgres;Password=12345"; 

// --- Реєстрація Сервісів ---
builder.Services.AddControllers(options =>
{
    // Централізована обробка помилок валідації (ProblemDetails - 2.00 балів)
    options.Filters.Add<ValidationExceptionMiddleware>(); 
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Football Store API (CQRS + EF Core)", Version = "v1" });
});

// 2. EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 3. AutoMapper, FluentValidation (2.00 балів)
builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);
builder.Services.AddFluentValidationAutoValidation();
// Реєстрація всіх валідаторів з шару Services (де знаходиться ProductCreateDtoValidator)
builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);

// 4. MediatR (CQRS, патерн Mediator - 0.50 балів)
builder.Services.AddMediatR(cfg => 
{
    // Реєстрація всіх Command/Query/Handler з шару Services
    cfg.RegisterServicesFromAssembly(typeof(ProductService).Assembly); 
    
    // Реєстрація CQRS пайплайна валідації (Validation with MediatR Pipeline - 2.00 балів)
    cfg.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
});

// 5. Реєстрація Repositories та Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductService, ProductService>(); 
// Повторна реєстрація Pipeline Behavior як IPipelineBehavior, оскільки MediatR не завжди реєструє його автоматично
builder.Services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>)); 


var app = builder.Build();

// --- АВТОМАТИЧНЕ ЗАСТОСУВАННЯ МІГРАЦІЙ (1.00 бал) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate(); 
        Log.Information("Database migration completed successfully.");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred during database migration.");
    }
}

// --- Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Football Store API v1");
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();