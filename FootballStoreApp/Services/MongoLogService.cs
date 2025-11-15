// Services/MongoLogService.cs
using MongoDB.Driver;
using FootballStoreApp.Models;
using System.Threading.Tasks;
using System;

namespace FootballStoreApp.Services
{
    public class MongoLogService
    {
        private readonly IMongoCollection<AuditLog> _logs;

        public MongoLogService(string connectionString, string databaseName, string collectionName)
        {
            try
            {
                var client = new MongoClient(connectionString);
                var database = client.GetDatabase(databaseName);
                _logs = database.GetCollection<AuditLog>(collectionName);
                Console.WriteLine($"\nMongoDB connection successful to DB: {databaseName}");
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\nWarning: Could not connect to MongoDB. Ensure mongod service is running. {ex.Message}");
                Console.ResetColor();
                _logs = null;
            }
        }

        public async Task LogActionAsync(string level, string message, string source = "System")
        {
            if (_logs == null) return;

            var log = new AuditLog
            {
                Level = level,
                Message = message,
                Source = source
            };

            await _logs.InsertOneAsync(log);
        }
    }
}