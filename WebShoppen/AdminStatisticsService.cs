using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShoppen.Models;

namespace WebShoppen
{
    public static class AdminStatisticsService
    {
        public static async Task ShowStatistics()
        {
            using var db = new AppDbContext();
            var sw = new Stopwatch();

            while (true)
            {
                Console.Clear();
                var options = new List<string>
            {
                "1. Best Selling Products",
                "2. Most Popular Category",
                "3. Orders Per City",
                "4. Monthly Revenue",
                "5. Back"
            };

                new Window("STATISTICS", 2, 2, options).Draw();
                Console.SetCursorPosition(2, options.Count + 4);

                switch (Console.ReadLine())
                {
                    case "1":
                        sw.Start();
                        await ShowTopProducts(db);
                        sw.Stop();
                        Console.WriteLine($"Query time: {sw.ElapsedMilliseconds}ms");
                        break;
                    case "2":
                        sw.Start();
                        await ShowPopularCategories(db);
                        sw.Stop();
                        Console.WriteLine($"Query time: {sw.ElapsedMilliseconds}ms");
                        break;
                    case "3":
                        sw.Start();
                        await ShowOrdersPerCity(db);
                        sw.Stop();
                        Console.WriteLine($"Query time: {sw.ElapsedMilliseconds}ms");
                        break;
                    case "4":
                        sw.Start();
                        await ShowMonthlyRevenue(db);
                        sw.Stop();
                        Console.WriteLine($"Query time: {sw.ElapsedMilliseconds}ms");
                        break;
                    case "5":
                        return;
                }
                Helper.PressKeyToContinue();
            }
        }

        private static async Task ShowTopProducts(AppDbContext db)
        {
            // Raw SQL example
            var sql = @"
            SELECT TOP 5 p.Id, p.Name, SUM(oi.Quantity) AS TotalSold
            FROM Products p
            JOIN OrderItems oi ON p.Id = oi.ProductId
            GROUP BY p.Id, p.Name
            ORDER BY TotalSold DESC";

            var results = await db.Database.SqlQueryRaw<ProductSalesDto>(sql).ToListAsync();
            DisplayResults(results.Select(r => $"{r.Name}: {r.TotalSold} sold"));
        }

        private static async Task ShowPopularCategories(AppDbContext db)
        {
            // LINQ example
            var results = await db.OrderItems
                .Include(oi => oi.Product.Category)
                .GroupBy(oi => oi.Product.Category.Name)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .ToListAsync();

            DisplayResults(results.Select(r => $"{r.Category}: {r.Count} orders"));
        }

        private static async Task ShowOrdersPerCity(AppDbContext db)
        {
            var sql = @"
            SELECT c.City, COUNT(o.Id) AS OrderCount
            FROM Customers c
            JOIN Orders o ON c.Id = o.CustomerId
            GROUP BY c.City
            ORDER BY OrderCount DESC";

            var results = await db.Database.SqlQueryRaw<CityOrdersDto>(sql).ToListAsync();
            DisplayResults(results.Select(r => $"{r.City}: {r.OrderCount} orders"));
        }

        private static async Task ShowMonthlyRevenue(AppDbContext db)
        {
            var sql = @"
            SELECT 
                YEAR(OrderDate) AS Year,
                MONTH(OrderDate) AS Month,
                SUM(Total + ShippingCost + VAT) AS Revenue
            FROM Orders
            GROUP BY YEAR(OrderDate), MONTH(OrderDate)
            ORDER BY Year DESC, Month DESC";

            var results = await db.Database.SqlQueryRaw<MonthlyRevenueDto>(sql).ToListAsync();
            DisplayResults(results.Select(r => $"{r.Year}-{r.Month}: {r.Revenue:C}"));
        }

        private static void DisplayResults(IEnumerable<string> results)
        {
            new Window("RESULTS", 40, 2, results.ToList()).Draw();
        }

        private class ProductSalesDto { public string Name { get; set; } public int TotalSold { get; set; } }
        private class CityOrdersDto { public string City { get; set; } public int OrderCount { get; set; } }
        private class MonthlyRevenueDto { public int Year { get; set; } public int Month { get; set; } public decimal Revenue { get; set; } }
    }
}

