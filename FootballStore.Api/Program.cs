using FootballStore.Data.Ef;
using FootballStore.Data.Ef.Repositories;
using FootballStore.Services;
using FootballStore.Services.Mapping;
using FootballStore.Services.Validation;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// --- Конфігурація EF Core та PostgreSQL ---
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ??
                       "Host=localhost;Database=football_db_ef;Username=postgres;Password=12345";

// --- Реєстрація Сервісів ---
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // OpenApiInfo тепер знайдено завдяки using Microsoft.OpenApi.Models
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Football Store API (EF Core)", Version = "v1" });
});

// 1. EF Core DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// 2. AutoMapper
builder.Services.AddAutoMapper(typeof(ProductProfile).Assembly);

// 3. FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(ProductCreateDtoValidator).Assembly);

// 4. Реєстрація Repositories та Services
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IProductService, ProductService>();


var app = builder.Build();

// --- Автоматичне застосування міграцій ---
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.Migrate(); 
}

// --- Централізована обробка помилок (ProblemDetails) ---
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