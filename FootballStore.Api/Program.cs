using FootballStore.Data.Ado;
using FootballStore.Data.Ado.Repositories;
using Npgsql;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// --- Конфігурація для PostgreSQL ---
// Оскільки ми використовуємо ADO.NET, нам потрібен тільки рядок підключення.
// Встановіть свій пароль
var connectionString = builder.Configuration.GetConnectionString("PostgresConnection") ?? 
"Host=localhost;Database=football_db;Username=postgres;Password=12345";

// --- Реєстрація Сервісів (IoC) ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 1. Swagger/OpenAPI документація (0.50 балів)
builder.Services.AddSwaggerGen(c =>
{
c.SwaggerDoc("v1", new OpenApiInfo { Title = "Football Store API (ADO.NET + Dapper)", Version = "v1" });
});

// 2. Реєстрація Unit of Work як Scoped Service
builder.Services.AddScoped<IUnitOfWork>(provider => new UnitOfWork(connectionString));

// Видаляємо стандартний контролер
// builder.Services.AddSingleton<FootballStore.Api.WeatherForecastController>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Football Store API V1");
    });
}

// app.UseHttpsRedirection(); // Зазвичай вимикаємо для локальної розробки

app.UseAuthorization();
app.MapControllers();

app.Run();