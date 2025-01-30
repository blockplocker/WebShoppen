using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using WebShoppen.Models;

namespace WebShoppen
{
    internal class DisplayShop
    {
        public static void ShowShopPage(User user)
        {
            using var db = new AppDbContext();

            while (true)
            {
                Console.Clear();
                var menuOptions = new List<string>
            {
                "1. Search Products",
                "2. Browse Categories",
                "3. View Cart",
                "4. Back to Main Menu"
            };

                new Window("SHOP MENU", 2, 2, menuOptions).Draw();
                Console.SetCursorPosition(2, menuOptions.Count + 4);

                int choice = Helper.GetValidIntegerMinMax("Select option: ", 1, 4);

                switch (choice)
                {
                    case 1:
                        ShowSearchProducts(user);
                        break;
                    case 2:
                        BrowseCategories(user);
                        break;
                    case 3:
                        ShowCartPage(user);
                        break;
                    case 4:
                        return;
                }
            }
        }

        private static void ShowSearchProducts(User user)
        {
            using var db = new AppDbContext();
            Console.Clear();
            new Window("PRODUCT SEARCH", 2, 2, new List<string>() {"                                                                                            " }).Draw();

            Console.SetCursorPosition(3, 3);
            Console.Write("Enter search term: ");
            string searchTerm = Console.ReadLine();

            var results = db.Products
                .Where(p => EF.Functions.Like(p.Name, $"%{searchTerm}%") ||
                            EF.Functions.Like(p.Description, $"%{searchTerm}%"))
                .Take(20)
                .ToList();

            ShowProductList(user, results,
                $"Search Results for '{searchTerm}'",
                $"No products found matching '{searchTerm}'");
        }

        private static void BrowseCategories(User user)
        {
            using var db = new AppDbContext();
            var categories = db.Categories.ToList();

            while (true)
            {
                Console.Clear();
                var categoryItems = categories
                    .Select((c, i) => $"{i + 1}. {c.Name}")
                    .Concat(new[] { $"{categories.Count + 1}. Back" })
                    .ToList();

                new Window("CATEGORIES", 2, 2, categoryItems).Draw();

                int choice = Helper.GetValidIntegerMinMax("Select category: ", 1, categories.Count + 1);

                if (choice == categories.Count + 1) break;

                var selectedCategory = categories[choice - 1];
                ShowCategoryProducts(user, selectedCategory);
            }
        }

        private static void ShowCategoryProducts(User user, Category category)
        {
            using var db = new AppDbContext();
            var products = db.Products
                .Where(p => p.CategoryId == category.Id)
                .Take(20)
                .ToList();

            ShowProductList(user, products,
                $"{category.Name} Products",
                $"No products found in {category.Name} category");
        }

        private static void ShowProductList(User user, List<Product> products, string title, string emptyMessage)
        {
            if (!products.Any())
            {
                new Window("INFO", 2, 2, new List<string> { emptyMessage }).Draw();
                Helper.PressKeyToContinue();
                return;
            }

            while (true)
            {
                Console.Clear();
                var productItems = products
                    .Select((p, i) => $"{i + 1}. {p.Name} - {p.Price:C}")
                    .Concat(new[] { $"{products.Count + 1}. Back" })
                    .ToList();

                new Window(title, 2, 2, productItems).Draw();

                int choice = Helper.GetValidIntegerMinMax("Select product: ", 1, products.Count + 1);

                if (choice == products.Count + 1) return;

                var selectedProduct = products[choice - 1];
                ShowProductDetails(user, selectedProduct);
            }
        }

        private static void ShowProductDetails(User user, Product product)
        {
            while (true)
            {
                Console.Clear();
                var details = new List<string>
            {
                $"Name: {product.Name}",
                $"Description: {product.Description}",
                $"Price: {product.Price:C}",
                $"Stock: {product.Stock}",
                "",
                "1. Add to Cart",
                "2. Back"
            };

                new Window("PRODUCT DETAILS", 2, 2, details).Draw();

                int choice = Helper.GetValidIntegerMinMax("Select option: ", 1, 2);

                if (choice == 1)
                {
                    int quantity = Helper.GetValidIntegerMinMax("Enter quantity: ", 1, 100);
                    ProductService.AddToCart(user, product.Id, quantity);
                    Console.WriteLine("Added to cart!");
                    Helper.PressKeyToContinue();
                }
                else
                {
                    return;
                }
            }
        }

        public static void ShowCartPage(User user)
        {
            using var db = new AppDbContext();

            // Ensure the user's cart is loaded from the database
            user.Cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) // Ensure products are loaded
                .FirstOrDefault(c => c.UserId == user.Id);

            while (true)
            {
                Console.Clear();

                if (user?.Cart == null || !user.Cart.Items.Any())
                {
                    var emptyCart = new List<string> { "Your cart is empty!" };
                    var windowCart = new Window("CART", 2, 2, emptyCart);
                    windowCart.Draw();
                    Helper.PressKeyToContinue();
                    return;
                }

                // Display cart items
                var cartItems = user.Cart.Items.Select(i => $"Id: {i.ProductId} - {i.Product.Name} x{i.Quantity} - {i.Product.Price * i.Quantity} SEK").ToList();
                var total = user.Cart.Items.Sum(i => i.Quantity * i.Product.Price);
                cartItems.Add($"Total: {total} SEK");

                var windowCartWithItems = new Window("CART", 2, 2, cartItems);
                windowCartWithItems.Draw();

                // Options for the user
                var options = new List<string>
        {
            "1. Change Quantity",
            "2. Remove Product",
            "3. Proceed to Shipping",
            "4. Back to Main Menu"
        };
                var windowOptions = new Window("OPTIONS", 2, cartItems.Count + 4, options);
                windowOptions.Draw();

                Console.SetCursorPosition(2, cartItems.Count + options.Count + 6);
                Console.Write("Enter your choice: ");
                int choice = Helper.GetValidInteger();

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter product ID to change quantity: ");
                        int productIdToChange = Helper.GetValidInteger();
                        var itemToChange = user.Cart.Items.FirstOrDefault(i => i.ProductId == productIdToChange);
                        if (itemToChange != null)
                        {
                            Console.Write("Enter new quantity: ");
                            int newQuantity = Helper.GetValidInteger();
                            if (newQuantity > 0)
                            {
                                itemToChange.Quantity = newQuantity;
                                db.SaveChanges();
                                Console.WriteLine("Quantity updated!");
                            }
                            else
                            {
                                Console.WriteLine("Quantity must be greater than 0.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("Product not found in cart.");
                        }
                        Helper.PressKeyToContinue();
                        break;

                    case 2:
                        Console.Write("Enter product ID to remove: ");
                        int productIdToRemove = Helper.GetValidInteger();
                        ProductService.RemoveFromCart(user, productIdToRemove);
                        Helper.PressKeyToContinue();
                        break;

                    case 3:
                        ShowShippingPage(user);
                        return;

                    case 4:
                        return;

                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        Helper.PressKeyToContinue();
                        break;
                }
            }
        }

        public static void ShowShippingPage(User user)
        {
            using var db = new AppDbContext();

            // Load or create customer
            var customer = db.Customers.FirstOrDefault(c => c.UserId == user.Id);
            bool isNewCustomer = customer == null;


            while (true)
            {
                Console.Clear();

                // Customer details form
                var customerDetails = new List<string>();
                if (isNewCustomer)
                {
                    customer = new Customer { UserId = user.Id };
                    customerDetails.Add("New Customer Details:");
                }
                else
                {
                    customerDetails.Add("Existing Customer Details:");
                    customerDetails.Add($"Name: {customer.FullName}");
                    customerDetails.Add($"Address: {customer.Address}");
                    customerDetails.Add($"Country: {customer.Country}");
                    customerDetails.Add($"City: {customer.City}");
                    customerDetails.Add($"Postal Code: {customer.PostalCode}");
                    customerDetails.Add($"Phone Number: {customer.Phone}");
                    customerDetails.Add($"Use existing details? (Y/N)");
                }

                var windowCustomer = new Window("CUSTOMER DETAILS", 2, 2, customerDetails);
                windowCustomer.Draw();

                if (!isNewCustomer)
                {
                    string choiceYesOrNo = Console.ReadKey().KeyChar.ToString().ToUpper();
                    if (choiceYesOrNo == "Y")
                    {
                        break; // Use existing details
                    }
                }

                // Collect/update details
                Console.SetCursorPosition(2, 13);
                Console.Write("Full Name: ");
                customer.FullName = Console.ReadLine();

                Console.SetCursorPosition(2, 14);
                Console.Write("Address: ");
                customer.Address = Console.ReadLine();

                Console.SetCursorPosition(2, 15);
                Console.Write("Country: ");
                customer.Country = Console.ReadLine();

                Console.SetCursorPosition(2, 16);
                Console.Write("City: ");
                customer.City = Console.ReadLine();

                Console.SetCursorPosition(2, 17);
                Console.Write("Postal Code: ");
                customer.PostalCode = Console.ReadLine();

                Console.SetCursorPosition(2, 18);
                Console.Write("Phone nummber: ");
                customer.Phone = Console.ReadLine();

                // Save changes and verify

                if (isNewCustomer)
                {
                    db.Customers.Add(customer);
                }
                db.SaveChanges();

                // Refresh customer to ensure ID is populated
                customer = db.Customers.First(c => c.UserId == user.Id);
                break;

            }
            // Shipping options
            var shippingOptions = new List<string> { $"1. Standard Shipping - {(int)ShippingOption.Standard} SEK", $"2. Express Shipping - {(int)ShippingOption.Express} SEK", $"3. Home Delivery - {(int)ShippingOption.HomeDelivery} SEK" };

            Console.Clear();
            var windowShipping = new Window("SHIPPING OPTIONS", 2, 2, shippingOptions);
            windowShipping.Draw();

            Console.SetCursorPosition(2, 8);
            Console.Write("Enter shipping option: ");
            int choice = Helper.GetValidInteger();

            int selectedShipping = choice == 1 ? (int)ShippingOption.Standard : choice == 2 ? (int)ShippingOption.Express : choice == 3 ? (int)ShippingOption.HomeDelivery : (int)ShippingOption.Standard;

            ShowPaymentPage(user, selectedShipping);
        }

        public static void ShowPaymentPage(User user, int shippingCost)
        {
            using var db = new AppDbContext();

            // Ensure the user's cart is loaded from the database
            user.Cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) // Ensure products are loaded
                .FirstOrDefault(c => c.UserId == user.Id);

            Console.Clear();

            // Display cart items and total
            var cartItems = user.Cart.Items.Select(i => $"{i.Product.Name} x{i.Quantity} - {i.Product.Price * i.Quantity} SEK").ToList();
            var total = user.Cart.Items.Sum(i => i.Quantity * i.Product.Price);
            var tax = (total + shippingCost) * 0.25m; // 25% tax
            var TotalPrice = total + shippingCost + tax;

            cartItems.Add($"Shipping: {shippingCost} SEK");
            cartItems.Add($"Tax: {tax} SEK");
            cartItems.Add($"Total Price: {TotalPrice} SEK");

            var windowCart = new Window("CART", 2, 2, cartItems);
            windowCart.Draw();

            // Payment options
            var paymentOptions = new List<string> { "1. Credit Card", "2. PayPal" };
            var windowPayment = new Window("PAYMENT OPTIONS", 2, cartItems.Count + 4, paymentOptions);
            windowPayment.Draw();

            Console.SetCursorPosition(2, cartItems.Count + paymentOptions.Count + 6);
            Console.Write("Enter payment method: ");
            int paymentMethod = Helper.GetValidInteger();

            // Place the order and clear the cart
            ProductService.Checkout(user, shippingCost);
        }
    }
}
