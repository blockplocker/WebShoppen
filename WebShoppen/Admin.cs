using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShoppen.Models;

namespace WebShoppen
{
    internal class Admin
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
                        AdminStatisticsService.ShowStatistics().Wait();
                        break;
                    case 6:
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void ManageProducts()
        {
            using var db = new AppDbContext();
            while (true)
            {
                Console.Clear();
                var productOptions = new List<string>
                {
                    "1. Add Product",
                    "2. Update Product",
                    "3. Delete Product",
                    "4. Make Product Featured",
                    "5. Back to Admin Panel"
                };
                var windowProducts = new Window("MANAGE PRODUCTS", 2, 2, productOptions);
                windowProducts.Draw();

                var products = db.Products.ToList();
                var productLines = products.Select(p => $"Id: {p.Id} - {p.Name} - {p.Price} kr - {p.Stock} Lager - {(p.IsFeatured ? "Featured" : "Not Featured")}").ToList();
                var windowProductList = new Window("PRODUCTS", 50, 2, productLines);
                windowProductList.Draw();

                var categories = db.Categories.ToList();
                var categoryLines = categories.Select(c => $"Id: {c.Id} - {c.Name}").ToList();
                var windowCategoryList = new Window("CATEGORIES", 30, 2, categoryLines);
                windowCategoryList.Draw();

                Console.SetCursorPosition(2, productOptions.Count + 4);
                int choice = Helper.GetValidInteger();

                switch (choice)
                {
                    case 1:
                        var product = new Product();
                        Console.Write("Product Name: ");
                        product.Name = Console.ReadLine();
                        Console.Write("Description: ");
                        product.Description = Console.ReadLine();
                        Console.Write("Price: ");
                        product.Price = Helper.GetValidDecimal();
                        Console.Write("Stock: ");
                        product.Stock = Helper.GetValidInteger();
                        Console.Write("Category ID: ");
                        product.CategoryId = Helper.GetValidInteger();
                        db.Products.Add(product);
                        db.SaveChanges();
                        Console.WriteLine("Product added successfully.");

                        MongoLogger.Log("ProductAdded", $"Added product: {product.Name}");
                        break;

                    case 2:
                        Console.Write("Enter Product ID to Update: ");
                        int productIdToUpdate = Helper.GetValidInteger();
                        var productToUpdate = db.Products.Find(productIdToUpdate);
                        if (productToUpdate != null)
                        {
                            Console.WriteLine($"Current Product Name: {productToUpdate.Name}");
                            Console.Write("New Product Name (leave empty to keep current): ");
                            var newName = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newName))
                            {
                                productToUpdate.Name = newName;
                            }

                            Console.WriteLine($"Current Description: {productToUpdate.Description}");
                            Console.Write("New Description (leave empty to keep current): ");
                            var newDescription = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newDescription))
                            {
                                productToUpdate.Description = newDescription;
                            }

                            Console.WriteLine($"Current Price: {productToUpdate.Price}");
                            Console.Write("New Price (leave empty to keep current): ");
                            var newPriceInput = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newPriceInput) && decimal.TryParse(newPriceInput, out var newPrice))
                            {
                                productToUpdate.Price = newPrice;
                            }

                            Console.WriteLine($"Current Stock: {productToUpdate.Stock}");
                            Console.Write("New Stock (leave empty to keep current): ");
                            var newStockInput = Console.ReadLine();
                            if (!string.IsNullOrEmpty(newStockInput) && int.TryParse(newStockInput, out var newStock))
                            {
                                productToUpdate.Stock = newStock;
                            }

                            db.SaveChanges();
                            Console.WriteLine("Product updated successfully.");

                            MongoLogger.Log("ProductUpdated", $"Updated product: {productToUpdate.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                        }
                        break;

                    case 3:
                        Console.Write("Enter Product ID to Delete: ");
                        int productIdToDelete = Helper.GetValidInteger();
                        var productToDelete = db.Products.Find(productIdToDelete);
                        if (productToDelete != null)
                        {
                            db.Products.Remove(productToDelete);
                            db.SaveChanges();
                            Console.WriteLine("Product deleted successfully.");

                            MongoLogger.Log("ProductDeleted", $"Deleted product ID: {productIdToDelete}");
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                        }
                        break;

                    case 4:
                        Console.Write("Enter Product ID to Make Featured: ");
                        int productIdToFeature = Helper.GetValidInteger();
                        var productToFeature = db.Products.Find(productIdToFeature);
                        if (productToFeature != null)
                        {
                            productToFeature.IsFeatured = !productToFeature.IsFeatured;
                            db.SaveChanges();
                            Console.WriteLine($"Product featured status changed to: {productToFeature.IsFeatured}");
                        }
                        else
                        {
                            Console.WriteLine("Product not found.");
                        }
                        break;

                    case 5:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
            }
        }

        private static void ManageCategories()
        {
            using var db = new AppDbContext();
            while (true)
            {
                Console.Clear();
                var categoryOptions = new List<string>
        {
            "1. Add Category",
            "2. Update Category",
            "3. Delete Category",
            "4. Back to Admin Panel"
        };
                var windowCategories = new Window("MANAGE CATEGORIES", 2, 2, categoryOptions);
                windowCategories.Draw();




                var products = db.Products.ToList();
                if (products.Any())
                {
                    var productLines = products.Select(p => $"Id: {p.Id} - {p.Name} - {p.Price} kr - {p.Stock} Lager").ToList();
                    var windowProductList = new Window("PRODUCTS", 50, 2, productLines);
                    windowProductList.Draw();
                }
                else
                {
                    var emptyCart = new List<string> { "There are no products!" };
                    var windowCart = new Window("PRODUCTS", 50, 2, emptyCart);
                    windowCart.Draw();
                }

                var categories = db.Categories.ToList();
                if (categories.Any())
                {
                    var categoryLines = categories.Select(c => $"Id: {c.Id} - {c.Name}").ToList();
                    var windowCategoryList = new Window("CATEGORIES", 30, 2, categoryLines);
                    windowCategoryList.Draw();
                }
                else
                {
                    var emptyCart = new List<string> { "There are no categories!" };
                    var windowCart = new Window("CATEGORIES", 30, 2, emptyCart);
                    windowCart.Draw();
                }

                Console.SetCursorPosition(2, categoryOptions.Count + 4);
                int choice = Helper.GetValidInteger();

                switch (choice)
                {
                    case 1:
                        var category = new Category();
                        Console.Write("Category Name: ");
                        category.Name = Console.ReadLine();
                        db.Categories.Add(category);
                        db.SaveChanges();
                        Console.WriteLine("Category added successfully.");

                        MongoLogger.Log("CategoryAdded", $"Added Category: {category.Name}");
                        break;

                    case 2:
                        Console.Write("Enter Category ID to Update: ");
                        int categoryIdToUpdate = Helper.GetValidInteger();
                        var categoryToUpdate = db.Categories.Find(categoryIdToUpdate);
                        if (categoryToUpdate != null)
                        {
                            Console.Write("New Category Name: ");
                            categoryToUpdate.Name = Console.ReadLine();
                            db.SaveChanges();
                            Console.WriteLine("Category updated successfully.");
                            MongoLogger.Log("CategoryUpdated", $"Updated Category: {categoryToUpdate.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Category not found.");
                        }
                        break;

                    case 3:
                        Console.Write("Enter Category ID to Delete: ");
                        int categoryIdToDelete = Helper.GetValidInteger();
                        var categoryToDelete = db.Categories.Find(categoryIdToDelete);
                        if (categoryToDelete != null)
                        {
                            db.Categories.Remove(categoryToDelete);
                            db.SaveChanges();
                            Console.WriteLine("Category deleted successfully.");

                            MongoLogger.Log("CategoryDeleted", $"Deleted Category: {categoryToDelete.Name}");
                        }
                        else
                        {
                            Console.WriteLine("Category not found.");
                        }
                        break;

                    case 4:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        break;
                }
                Helper.PressKeyToContinue();
            }
        }

        private static void ManageUsers()
        {
            using var db = new AppDbContext();
            while (true)
            {
                Console.Clear();
                var userOptions = new List<string>
        {
            "1. Edit Customer Details",
            "2. View Order History",
            "3. Delete User",
            "4. Change User Admin Status",
            "5. Change User Password",
            "6. Back to Admin Panel"
        };

                var windowUsers = new Window("MANAGE USERS", 2, 2, userOptions);
                windowUsers.Draw();

                var users = db.Users.ToList();
                var userLines = users.Select(u => $"Id: {u.Id} {u.Username} {(u.IsAdmin ? "Admin:" : "User:")} ").ToList();

                new Window("USER LIST", 40, 2, userLines).Draw();

                Console.SetCursorPosition(2, userOptions.Count + 4);
                int choice = Helper.GetValidIntegerMinMax("Select option: ", 1, 6);

                switch (choice)
                {
                    case 1: // Edit Customer Details
                        EditCustomerDetails();
                        break;

                    case 2: // View Order History
                        ViewOrderHistory();
                        break;

                    case 3: // Delete User
                        DeleteUser();
                        break;

                    case 4: // Change Admin Status
                        ToggleAdminStatus();
                        break;

                    case 5: // Change Password
                        ChangeUserPassword();
                        break;

                    case 6: // Exit
                        return;
                }
                Helper.PressKeyToContinue();
            }
        }

        private static void EditCustomerDetails()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();

            var user = db.Users
                .Include(u => u.Customer)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            if (user.Customer == null)
            {
                Console.WriteLine("No customer details exist. Create new? (Y/N)");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                    return;

                user.Customer = new Customer { UserId = user.Id };
            }

            EditCustomerField("Full Name", user.Customer.FullName, v => user.Customer.FullName = v);
            EditCustomerField("Address", user.Customer.Address, v => user.Customer.Address = v);
            EditCustomerField("City", user.Customer.City, v => user.Customer.City = v);
            EditCustomerField("Postal Code", user.Customer.PostalCode, v => user.Customer.PostalCode = v);
            EditCustomerField("Country", user.Customer.Country, v => user.Customer.Country = v);
            EditCustomerField("Phone", user.Customer.Phone, v => user.Customer.Phone = v);

            db.SaveChanges();
            Console.WriteLine("Customer details updated!");
        }

        private static void EditCustomerField(string fieldName, string currentValue, Action<string> setter)
        {
            Console.Write($"{fieldName} ({currentValue}): ");
            var newValue = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newValue))
            {
                setter(newValue);
            }
        }

        private static void ViewOrderHistory()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();

            var customer = db.Customers
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                Console.WriteLine("No customer details found!");
                return;
            }

            var orders = db.Orders
                .Where(o => o.CustomerId == customer.Id)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var orderLines = new List<string>();
            foreach (var order in orders)
            {
                orderLines.Add($"Order #{order.Id} - {order.OrderDate:yyyy-MM-dd}");
                orderLines.Add($"Total: {order.Total + order.ShippingCost + order.VAT:C}");
                orderLines.AddRange(order.Items.Select(i =>
                    $"{i.Product.Name} x{i.Quantity} @ {i.Product.Price:C}"
                ));
                orderLines.Add("");
            }

            new Window($"ORDER HISTORY FOR {customer.FullName}", 2, 2, orderLines).Draw();
        }

        private static void DeleteUser()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID to Delete: ");
            int userId = Helper.GetValidInteger();
            var user = db.Users.Find(userId);

            if (user != null)
            {
                MongoLogger.Log("UserDeleted", $"Deleted user: {user.Username}");

                db.Users.Remove(user);
                db.SaveChanges();
                Console.WriteLine("User deleted successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private static void ToggleAdminStatus()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();
            var user = db.Users.Find(userId);

            if (user != null)
            {
                user.IsAdmin = !user.IsAdmin;
                db.SaveChanges();
                Console.WriteLine($"Admin status changed to: {user.IsAdmin}");

                MongoLogger.Log("AdminStatusChanged", $"User {user.Username} status: {user.IsAdmin}");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private static void ChangeUserPassword()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();
            var user = db.Users.Find(userId);

            AuthService.ChangePassword(user);
        }


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
