using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace FootballStoreApp.Models
{
    public class AuditLog
    {
        // ObjectId - стандартний первинний ключ MongoDB
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        // Рівень логування (Error, Info, Warning). Використовуємо required для .NET 9.
        public required string Level { get; set; }
        
        // Деталі повідомлення
        public required string Message { get; set; }
        
        // Користувач або сутність, яка ініціювала дію
        public required string Source { get; set; }
    }
}