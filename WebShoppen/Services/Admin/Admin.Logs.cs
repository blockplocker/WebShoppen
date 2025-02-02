using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShoppen.Models;

namespace WebShoppen.Services.Admin
{
    internal partial class Admin
    {
        private static void ShowActivityLogs()
        {
            using var db = new AppDbContext();
            var filterOptions = new List<string>
    {
        "1. All Activity",
        "2. Order Activities",
        "3. Login Activities",
        "4. Product Activities",
        "5. Category Activities",
        "6. User Activities",
        "7. Back"
    };

            while (true)
            {
                Console.Clear();
                new Window("ACTIVITY LOGS", 2, 2, filterOptions).Draw();

                int choice = Helper.GetValidIntegerMinMax("Select log type: ", 1, 7);
                if (choice == 7) return;

                var logs = GetLogsByType(choice);
                DisplayLogs(logs);
            }
        }

        private static List<BsonDocument> GetLogsByType(int choice)
        {
            var filterBuilder = Builders<BsonDocument>.Filter;
            FilterDefinition<BsonDocument> filter = choice switch
            {
                1 => filterBuilder.Empty,
                2 => filterBuilder.Regex("action", new BsonRegularExpression("Order")),
                3 => filterBuilder.Or(
                        filterBuilder.Eq("action", "UserLogin"),
                        filterBuilder.Eq("action", "FailedLogin"),
                        filterBuilder.Eq("action", "UserRegistration")),
                4 => filterBuilder.Regex("action", new BsonRegularExpression("Product")),
                5 => filterBuilder.Regex("action", new BsonRegularExpression("Category")),
                6 => filterBuilder.Regex("action", new BsonRegularExpression("User|Admin")),
                _ => filterBuilder.Empty
            };

            return MongoLogger.GetLogs(filter);
        }

        private static void DisplayLogs(List<BsonDocument> logs)
        {
            var logLines = new List<string>();

            foreach (var log in logs.OrderByDescending(l => l["timestamp"]))
            {
                var timestamp = log["timestamp"].ToUniversalTime().ToString("yyyy-MM-dd HH:mm");
                var action = log["action"].AsString.PadRight(20);
                var details = log["details"];

                logLines.Add($"{timestamp} | {action} | {details}");
            }

            if (!logLines.Any())
            {
                logLines.Add("No logs found for this category");
            }

            new Window("LOGS", 2, 6, logLines).Draw();
            Helper.PressKeyToContinue();
        }
    }
}
