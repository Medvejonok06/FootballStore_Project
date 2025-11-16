using MongoDB.Driver;
using FootballStoreApp.Models;
using System.Threading.Tasks;
using System;
using MongoDB.Bson; // <-- НОВИЙ USING
using System.Collections.Generic;
using System.Linq;

namespace FootballStoreApp.Services
{
    public class MongoLogService
    {
        private readonly IMongoCollection<AuditLog> _logs;
        
        // ... (Конструктор та LogActionAsync залишаються без змін) ...
        
        public MongoLogService(string connectionString, string databaseName, string collectionName)
        {
            // ... (існуючий код конструктора) ...
        }

        public async Task LogActionAsync(string level, string message, string source = "System")
        {
            // ... (існуючий код LogActionAsync) ...
        }

        // --- MongoDB репозиторії з aggregation (1.00 бал) ---
        public async Task<List<BsonDocument>> GetLogsByAggregation(string source)
        {
            if (_logs == null) return new List<BsonDocument>();

            // Pipeline для демонстрації Aggregation: 
            // 1. Фільтруємо за Source
            // 2. Групуємо за рівнем логування (Level)
            // 3. Підраховуємо кількість
            var pipeline = new BsonDocument[]
            {
                new BsonDocument("$match", new BsonDocument("Source", source)),
                new BsonDocument("$group", 
                    new BsonDocument 
                    {
                        { "_id", "$Level" }, // Групуємо за рівнем
                        { "count", new BsonDocument("$sum", 1) } // Підраховуємо
                    })
            };

            // Виконуємо Aggregation Pipeline
            var aggregationResult = await _logs.AggregateAsync<BsonDocument>(pipeline);

            return await aggregationResult.ToListAsync();
        }
    }
}