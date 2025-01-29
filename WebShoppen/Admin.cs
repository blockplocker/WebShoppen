using Microsoft.EntityFrameworkCore;
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
                var adminOptions = new List<string> { "1. Manage Products", "2. Manage Categories", "3. Manage Users", "4. Exit" };
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
                var productLines = products.Select(p => $"Id: {p.Id} - {p.Name} - {p.Price} kr - {p.Stock} Lager").ToList();
                var windowProductList = new Window("PRODUCTS", 50, 2, productLines);
                windowProductList.Draw();

                var categories = db.Categories.ToList();
                var categoryLines = categories.Select(c => $"Id: {c.Id} - {c.Name}").ToList();
                var windowCategoryList = new Window("CATEGORIES", 30, 2, categoryLines);
                windowCategoryList.Draw();

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
                    "1. Delete User",
                    "2. Change User Admin Status",
                    "3. Change User Password",
                    "4. Back to Admin Panel"
                };
                var windowUsers = new Window("MANAGE USERS", 2, 2, userOptions);
                windowUsers.Draw();

                var users = db.Users.ToList();
                var userLines = users.Select(u => $"Id: {u.Id} {u.Username} {(u.IsAdmin ? "Admin:" : "User:")} ").ToList();
                var windowUserList = new Window("USER LIST", 40, 2, userLines);
                windowUserList.Draw();

                Console.SetCursorPosition(2, userOptions.Count + 4);
                int choice = Helper.GetValidInteger();

                switch (choice)
                {

                    case 1:
                        Console.Write("Enter User ID to Delete: ");
                        int userIdToDelete = Helper.GetValidInteger();
                        var userToDelete = db.Users.Find(userIdToDelete);
                        if (userToDelete != null)
                        {
                            db.Users.Remove(userToDelete);
                            db.SaveChanges();
                            Console.WriteLine("User deleted successfully.");
                        }
                        else
                        {
                            Console.WriteLine("User not found.");
                        }
                        break;

                    case 2:
                        Console.Write("Enter User ID to Change Admin Status: ");
                        int userIdToChange = Helper.GetValidInteger();
                        var userToChange = db.Users.Find(userIdToChange);
                        if (userToChange != null)
                        {
                            userToChange.IsAdmin = !userToChange.IsAdmin;
                            db.SaveChanges();
                            Console.WriteLine($"User admin status changed to: {userToChange.IsAdmin}");
                        }
                        else
                        {
                            Console.WriteLine("User not found.");
                        }
                        break;

                    case 3: 
                        Console.Write("Enter User ID to Change Password: ");
                        int userIdToChangePassword = Helper.GetValidInteger();
                        var userToChangePassword = db.Users.Find(userIdToChangePassword);
                        AuthService.ChangePassword(userToChangePassword);
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


    }
}
