using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShoppen.Models;

namespace WebShoppen
{
    internal class DisplayShop
    {








        public static void ShowHomePage()
        {
            Console.Clear();
            using var db = new AppDbContext();
            var welcomeText = new List<string>
    {
        "Welcome to Our Webshop!",
        "Find the best products at great prices!"
    };
            var windowWelcome = new Window("HOME", 2, 2, welcomeText);
            windowWelcome.Draw();

            var featuredProducts = db.Products.Where(p => p.IsFeatured).Take(3).ToList();

            if (featuredProducts.Count > 0)
            {
                var Product1 = featuredProducts[0];
                var featuredProductList1 = new List<string> { Product1.Name, Product1.Description, "Price: " + Product1.Price + " kr", "Press A to buy" };
                var windowFeatured1 = new Window("FEATURED PRODUCT 1", 2, 7, featuredProductList1);
                windowFeatured1.Draw();
            }

            if (featuredProducts.Count > 1)
            {
                var Product2 = featuredProducts[1];
                var featuredProductList2 = new List<string> { Product2.Name, Product2.Description, "Price: " + Product2.Price + " kr", "Press B to buy" };
                var windowFeatured2 = new Window("FEATURED PRODUCT 2", 30, 7, featuredProductList2);
                windowFeatured2.Draw();
            }

            if (featuredProducts.Count > 2)
            {
                var Product3 = featuredProducts[2];
                var featuredProductList3 = new List<string> { Product3.Name, Product3.Description, "Price: " + Product3.Price + " kr", "Press C to buy" };
                var windowFeatured3 = new Window("FEATURED PRODUCT 3", 58, 7, featuredProductList3);
                windowFeatured3.Draw();
            }

            Console.SetCursorPosition(2, 13);
            Helper.PressKeyToContinue();
        }

        public static void ShowShopPage(User user)
        {
            using var db = new AppDbContext();
            while (true)
            {
                Console.Clear();

                // Display categories
                var categories = db.Categories.ToList();
                var categoryList = categories.Select(c => $"{c.Id}. {c.Name}").ToList();
                var windowCategories = new Window("SHOP CATEGORIES", 2, 2, categoryList);
                windowCategories.Draw();

                Console.SetCursorPosition(2, categoryList.Count + 4);
                Console.Write("Enter category ID to browse (0 to exit): ");
                int categoryId = Helper.GetValidInteger();

                if (categoryId == 0) break;

                // Display products in the selected category
                var products = db.Products.Where(p => p.CategoryId == categoryId).ToList();
                var productList = products.Select(p => $"{p.Id}. {p.Name} - {p.Price} SEK").ToList();
                var windowProducts = new Window("PRODUCTS", 2, 8, productList);
                windowProducts.Draw();

                Console.SetCursorPosition(2, 14);
                Console.Write("Enter product ID to view details (0 to go back): ");
                int productId = Helper.GetValidInteger();

                if (productId == 0) continue;

                ShowProductDetails(user, productId);
            }
        }

        public static void ShowProductDetails(User user, int productId)
        {
            using var db = new AppDbContext();
            var product = db.Products.Find(productId);
            if (product == null)
            {
                Console.WriteLine("Product not found.");
                Helper.PressKeyToContinue();
                return;
            }

            var details = new List<string>
    {
        $"Name: {product.Name}",
        $"Description: {product.Description}",
        $"Price: {product.Price} SEK"
    };
            var windowDetails = new Window("PRODUCT DETAILS", 2, 2, details);
            windowDetails.Draw();

            Console.SetCursorPosition(2, 8);
            Console.Write("Enter quantity to purchase (0 to cancel): ");
            int quantity = Helper.GetValidInteger();

            if (quantity > 0)
            {
                ProductService.AddToCart(user, productId, quantity);
                Console.WriteLine("Product added to cart!");
                Helper.PressKeyToContinue();
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
                var cartItems = user.Cart.Items.Select(i => $"Id: {i.Id} - {i.Product.Name} x{i.Quantity} - {i.Product.Price * i.Quantity} SEK").ToList();
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
                    customerDetails.Add($"Use existing details? (Y/N)");
                }

                var windowCustomer = new Window("CUSTOMER DETAILS", 2, 2, customerDetails);
                windowCustomer.Draw();

                if (!isNewCustomer)
                {
                    var choice = Console.ReadKey().KeyChar.ToString().ToUpper();
                    if (choice == "Y")
                    {
                        break; // Use existing details
                    }
                }

                // Collect/update details
                Console.SetCursorPosition(2, 8);
                Console.Write("Full Name: ");
                customer.FullName = Console.ReadLine();

                Console.SetCursorPosition(2, 9);
                Console.Write("Address: ");
                customer.Address = Console.ReadLine();

                Console.SetCursorPosition(2, 10);
                Console.Write("City: ");
                customer.City = Console.ReadLine();

                Console.SetCursorPosition(2, 11);
                Console.Write("Postal Code: ");
                customer.PostalCode = Console.ReadLine();

                if (isNewCustomer)
                {
                    db.Customers.Add(customer);
                }
                else
                {
                    db.Customers.Update(customer);
                }
                db.SaveChanges();
                break;
            }
            // Shipping options
            var shippingOptions = new List<string> { "1. Standard Shipping - 50 SEK", "2. Express Shipping - 100 SEK" };

            Console.Clear();
            var windowShipping = new Window("SHIPPING OPTIONS", 2, 2, shippingOptions);
            windowShipping.Draw();

            Console.SetCursorPosition(2, 6);
            Console.Write("Enter shipping option: ");
            int shippingCost = Console.ReadLine() == "1" ? 50 : 100;

            ShowPaymentPage(user, shippingCost);
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
            Console.WriteLine("Payment successful! Your order has been placed.");
            Helper.PressKeyToContinue();
        }
    }
}
