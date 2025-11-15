using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FootballStoreApp.Data;
using FootballStoreApp.Models;
using FootballStoreApp.Services;

// --- 1. КОНФІГУРАЦІЯ ТРЬОХ БД (Мікросервісна ізоляція) ---
// PostgreSQL (SQL): Products, Orders (PostgreSQL Context)
const string PgConnectionString = "Host=localhost;Database=football_db;Username=postgres;Password=12345"; 
// Oracle (SQL): Customers (Oracle Context)
const string OraConnectionString = "User Id=SYSTEM;Password=12345;Data Source=localhost:1521/XE";
// MongoDB (NoSQL): Logs (MongoLogService)
const string MongoConnectionString = "mongodb://localhost:27017"; 
const string MongoDbName = "FootballStoreAudit";
const string MongoCollectionName = "Logs";


// --- 2. НАЛАШТУВАННЯ IoC Контейнера ---
var serviceCollection = new ServiceCollection()
    // PostgreSQL Context
    .AddDbContext<ApplicationDbContext>(options =>
        options.UseNpgsql(PgConnectionString))
    // Oracle Context
    .AddDbContext<OracleDbContext>(options =>
        options.UseOracle(OraConnectionString))
    // MongoDB Service
    .AddSingleton(new MongoLogService(MongoConnectionString, MongoDbName, MongoCollectionName));

var serviceProvider = serviceCollection.BuildServiceProvider();


// --- 3. ВИКОНАННЯ ЛОГІКИ ---
using (var scope = serviceProvider.CreateScope())
{
    var pgContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var oraContext = scope.ServiceProvider.GetRequiredService<OracleDbContext>(); // КОНТЕКСТ ORACLE
    var logService = scope.ServiceProvider.GetRequiredService<MongoLogService>(); 
    
    // 3.1. СТВОРЕННЯ СХЕМ (PostgreSQL)
    Console.WriteLine("Ensuring PostgreSQL (Products, Orders) schema is created...");
    pgContext.Database.EnsureDeleted(); // Видалення для чистого старту
    pgContext.Database.EnsureCreated(); // Створення схеми
    Console.WriteLine("PostgreSQL schema is ready.");

    // 3.2. СТВОРЕННЯ СХЕМ (Oracle)
    Console.WriteLine("Ensuring Oracle (Customers) schema is created...");
    oraContext.Database.EnsureDeleted(); // Видалення для чистого старту
    oraContext.Database.EnsureCreated(); // Створення схеми
    Console.WriteLine("Oracle schema is ready.");

    // Додавання початкових даних (Тільки Product)
    SeedData(pgContext);

    // Запуск основної логіки
    await RunStoreOperations(pgContext, oraContext, logService); 
}

Console.WriteLine("Application finished.");


// =========================================================
//                   ДОПОМІЖНІ ФУНКЦІЇ
// =========================================================

// --- Функція для додавання початкових даних ---
void SeedData(ApplicationDbContext context)
{
    // ... (код SeedData залишається незмінним)
    if (!context.Products.Any())
    {
        Console.WriteLine("Adding initial seed data...");
        context.Products.AddRange(
            new Product { Name = "Футболка Збірна України", Description = "Домашня форма 2024", Price = 1200.00m, StockQuantity = 50 },
            new Product { Name = "М'яч Adidas Roteiro", Description = "Офіційний м'яч Євро-2004", Price = 3500.50m, StockQuantity = 5 },
            new Product { Name = "Шарф ФК Динамо Київ", Description = "Вболівальницький шарф", Price = 450.00m, StockQuantity = 100 }
        );
        context.SaveChanges();
        Console.WriteLine("Seed data added.");
    }
    else
    {
        Console.WriteLine("Database already contains data. Skipping seeding.");
    }
}


// --- Основна функція для виконання операцій (з трьома контекстами) ---
async Task RunStoreOperations(ApplicationDbContext pgContext, OracleDbContext oraContext, MongoLogService logService)
{
    Console.WriteLine("\nRunning CRUD and Business operations across 3 DBs...");
    
    // ЛОГУВАННЯ: Початок роботи (MongoDB)
    await logService.LogActionAsync("INFO", "Starting store operation cycle.", "AppStartup");
    
    // --- 1. ЧИТАННЯ (READ) ---
    Console.WriteLine("\n--- 1. Доступні товари ---");
    var productsList = await pgContext.Products.ToListAsync();
    foreach (var p in productsList)
    {
        Console.WriteLine($"ID: {p.Id}, Назва: {p.Name}, Ціна: {p.Price:C}, К-сть: {p.StockQuantity}");
    }
    
    // --- 2. ОНОВЛЕННЯ (UPDATE - приклад) ---
    var ball = await pgContext.Products
        .FirstOrDefaultAsync(p => p.Name.Contains("М'яч")); 
    if (ball != null)
    {
        ball.Price = 3700.00m;
        await pgContext.SaveChangesAsync(); 
        Console.WriteLine($"\nМ'яч оновлено. Нова ціна: {ball.Price:C}");
    }

    // --- 3. БІЗНЕС-ЛОГІКА: УСПІШНЕ ЗАМОВЛЕННЯ (ТРАНЗАКЦІЯ) ---
    await PlaceOrder(pgContext, oraContext, logService, "ivan.koval@test.com", new List<(string productName, int quantity)>
    {
        ("Футболка Збірна України", 2), 
        ("Шарф ФК Динамо Київ", 1) 
    });

    // --- 4. БІЗНЕС-ЛОГІКА: НЕУСПІШНЕ ЗАМОВЛЕННЯ (ПЕРЕВІРКА ROLLBACK) ---
    await PlaceOrder(pgContext, oraContext, logService, "error@test.com", new List<(string productName, int quantity)>
    {
        ("Футболка Збірна України", 100) 
    });
    
    // --- 5. ФІНАЛЬНА ПЕРЕВІРКА ЗАЛИШКІВ ---
    Console.WriteLine("\n--- Фінальна перевірка залишків ---");
    var finalShirtStock = await pgContext.Products
        .Where(p => p.Name == "Футболка Збірна України")
        .Select(p => p.StockQuantity)
        .FirstAsync();
        
    Console.WriteLine($"Залишок 'Футболка Збірна України' після транзакцій: {finalShirtStock}"); 
}


// --- Функція для обробки повного замовлення (Транзакція PGSQL + Customer з Oracle) ---
async Task PlaceOrder(ApplicationDbContext pgContext, OracleDbContext oraContext, MongoLogService logService, string customerEmail, List<(string productName, int quantity)> items)
{
    // Транзакція для PGSQL (Orders, Products)
    using (var transaction = await pgContext.Database.BeginTransactionAsync())
    {
        try
        {
            Console.WriteLine($"\n--- Оформлення замовлення для {customerEmail} ---");

            // 1. ЗНАХОДИМО АБО СТВОРЮЄМО КЛІЄНТА (ОПЕРАЦІЯ В ORACLE)
            var customer = await oraContext.Customers
                .FirstOrDefaultAsync(c => c.Email == customerEmail);

            if (customer == null)
            {
                customer = new Customer { FirstName = "Test", LastName = "Customer", Email = customerEmail };
                oraContext.Customers.Add(customer);
                await oraContext.SaveChangesAsync(); // ЗБЕРІГАННЯ В ORACLE
                Console.WriteLine($"Клієнта створено в Oracle (ID: {customer.Id}).");
            }

            // 2. СТВОРЮЄМО НОВЕ ЗАМОВЛЕННЯ (ОПЕРАЦІЯ В PGSQL)
            var newOrder = new Order { CustomerId = customer.Id, TotalAmount = 0.00m };
            pgContext.Orders.Add(newOrder);
            await pgContext.SaveChangesAsync(); 
            
            decimal orderTotal = 0;

            // 3. ОБРОБКА ПОЗИЦІЙ ТА ПЕРЕВІРКА ЗАЛИШКІВ (ОПЕРАЦІЯ В PGSQL)
            foreach (var item in items)
            {
                var product = await pgContext.Products
                    .FirstOrDefaultAsync(p => p.Name == item.productName);

                if (product == null || product.StockQuantity < item.quantity)
                {
                    // ВИКЛИКАЄМО ПОМИЛКУ ДЛЯ ROLLBACK
                    throw new Exception($"Недостатньо або не знайдено товару '{item.productName}'. На складі: {(product?.StockQuantity ?? 0)}");
                }

                // ЗМЕНШЕННЯ ЗАЛИШКІВ
                product.StockQuantity -= item.quantity;
                pgContext.Products.Update(product);

                // СТВОРЕННЯ ПОЗИЦІЇ
                var orderItem = new OrderItem
                {
                    OrderId = newOrder.Id,
                    ProductId = product.Id,
                    Quantity = item.quantity,
                    PricePerUnit = product.Price 
                };

                pgContext.OrderItems.Add(orderItem);
                orderTotal += orderItem.PricePerUnit * item.quantity;

                Console.WriteLine($" - Додано: {item.productName} x {item.quantity}. Залишок оновлено до {product.StockQuantity}.");
            }

            // 4. ОНОВЛЕННЯ СУМИ
            newOrder.TotalAmount = orderTotal;
            pgContext.Orders.Update(newOrder);
            await pgContext.SaveChangesAsync();

            // 5. КОМІТ (ПІДТВЕРДЖЕННЯ) ТРАНЗАКЦІЇ (PGSQL)
            await transaction.CommitAsync();
            Console.WriteLine($"Замовлення (ID: {newOrder.Id}) успішно оформлено. Загальна сума: {newOrder.TotalAmount:C}");
            
            // ЛОГУВАННЯ (MongoDB)
            await logService.LogActionAsync("SUCCESS", $"Order {newOrder.Id} created. Customer retrieved from Oracle.", customerEmail);
        }
        catch (Exception ex)
        {
            // ВІДКОТ ТРАНЗАКЦІЇ (PGSQL)
            await transaction.RollbackAsync();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nПОМИЛКА ЗАМОВЛЕННЯ. Транзакція PGSQL відкочена. {ex.Message}");
            
            // ЛОГУВАННЯ ПОМИЛКИ (MongoDB)
            await logService.LogActionAsync("ERROR", $"Transaction rollback during order attempt. Reason: {ex.Message}", customerEmail);
            
            Console.ResetColor();
        }
    }
}