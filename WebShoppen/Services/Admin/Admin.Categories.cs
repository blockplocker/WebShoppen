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
    }
}
