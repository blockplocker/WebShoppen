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
    }
}
