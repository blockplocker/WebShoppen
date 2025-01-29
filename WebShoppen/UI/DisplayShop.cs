//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;


//namespace WebShoppen
//{
//    internal class DisplayShop
//    {

//        public static async Task DisplayFromDatabaseAsync()
//        {
//            List<string> topText = new List<string> { "# Fina butiken #", "Fina kläder för låga priser" };
//            var windowTop = new Window("", 2, 1, topText);
//            windowTop.Draw();
//            try
//            {
//                using (var context = new Models.AppDbContext())
//                {
//                    // Fetch categories asynchronously
//                    var categories = await context.ProductCategories
//                        .Select(c => $"{c.Id}. {c.CategoryName}")
//                        .ToListAsync();

//                    // Fetch cart items asynchronously
//                    var cartItems = await context.Products
//                        .Where(p => p.MarkedProduct) // Example filter for marked products
//                        .Select(p => $"1 st {p.ProductName}")
//                        .ToListAsync();

//                    // Fetch top-selling products asynchronously
//                    var topSellingProducts = await context.Products
//                        .OrderByDescending(p => p.Price) // Example sort for demo
//                        .Take(5)
//                        .Select(p => $"{p.ProductName} - {p.Price:C}")
//                        .ToListAsync();

//                    // Display data in windows
//                    var windowCategories = new Window("Kategorier", 2, 6, categories);
//                    windowCategories.Draw();

//                    var windowCart = new Window("Din varukorg", 30, 6, cartItems);
//                    windowCart.Draw();

//                    var windowTopProducts = new Window("Bäst säljande produkter", 2, 15, topSellingProducts);
//                    windowTopProducts.Draw();
//                }
//            }
//            catch (Exception ex)
//            {
//                // Log the error and provide feedback to the user
//                Console.WriteLine($"An error occurred while retrieving data: {ex.Message}");
//            }
//        }
//    }
//}
