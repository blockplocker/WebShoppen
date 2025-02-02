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
        public static void AdminPanel()
        {
            using var db = new AppDbContext();
            while (true)
            {
                Console.Clear();
                var adminOptions = new List<string> { "1. Manage Products", "2. Manage Categories", "3. Manage Users", "4. View Activity Logs", "5. Statistics", "6. Exit" };
                var windowAdmin = new Window("ADMIN PANEL", 2, 2, adminOptions);
                windowAdmin.Draw();

                var products = db.Products.ToList();
                var productLines = products.Select(p => $"Id: {p.Id} - {p.Name} - {p.Price} kr - {p.Stock} Lager").ToList();
                var windowProductList = new Window("PRODUCTS", 50, 2, productLines);
                windowProductList.Draw();

                var categories = db.Categories.ToList();
                var categoryLines = categories.Select(c => $"Id: {c.Id} - {c.Name}").ToList();
                var windowCategoryList = new Window("CATEGORIES", 30, 2, categoryLines);
                windowCategoryList.Draw();

                Console.SetCursorPosition(2, adminOptions.Count + 4);
                int choice = Helper.GetValidInteger();

                switch (choice)
                {
                    case 1:
                        ManageProducts();
                        break;
                    case 2:
                        ManageCategories();
                        break;
                    case 3:
                        ManageUsers();
                        break;
                    case 4:
                        ShowActivityLogs();
                        break;
                    case 5:
                        ShowStatistics().Wait();
                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }
    }
}
