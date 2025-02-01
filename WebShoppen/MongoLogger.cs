using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen
{
  
public static class MongoLogger
    {
        private static readonly IMongoDatabase _database;
        private static readonly IMongoCollection<BsonDocument> _logs;

        static MongoLogger()
        {
            var client = new MongoClient("mongodb+srv://noadb:Hemligt123@cluster0.tyg2x.mongodb.net/?retryWrites=true&w=majority&appName=Cluster0");
            _database = client.GetDatabase("WebshopLogs");
            _logs = _database.GetCollection<BsonDocument>("ActivityLogs");
        }

        public static void Log(string action, string details)
        {
            var logEntry = new BsonDocument
            {
                { "timestamp", DateTime.UtcNow },
                { "action", action },
                { "details", details }                
            };

            _logs.InsertOne(logEntry);
        }

        public static List<BsonDocument> GetLogs(FilterDefinition<BsonDocument> filter, int limit = 50)
        {
            return _logs.Find(filter)
                .SortByDescending(l => l["timestamp"])
                .Limit(limit)
                .ToList();
        }
    }
}

