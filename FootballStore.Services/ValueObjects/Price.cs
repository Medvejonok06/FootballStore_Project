using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;
using System;
using MongoDB.Bson;

namespace FootballStore.Services.ValueObjects
{
    // Value Objects з BSON серіалізацією (1.00 бал)
    public class Price : ValueObject
    {
        // Виправлення CS0246
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Value { get; }
        public required string Currency { get; init; } = "UAH"; // Наприклад, валюта

        // Виправлення CS9035 (вимога C# 11 'required' для Value Object)
        public Price(decimal value, string currency = "UAH")
        {
            if (value < 0)
                throw new ArgumentException("Price value cannot be negative.", nameof(value));
            
            Value = value;
            Currency = currency;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
            yield return Currency;
        }
        
        // ... (решта коду ValueObject)
    }
}