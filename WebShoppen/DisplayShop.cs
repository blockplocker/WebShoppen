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

                var cartItems = user.Cart.Items.Select(i => $"{i.Product.Name} x{i.Quantity} - {i.Product.Price * i.Quantity} SEK").ToList();
                var windowCartWithItems = new Window("CART", 2, 2, cartItems);
                windowCartWithItems.Draw();

                Console.SetCursorPosition(2, cartItems.Count + 4);
                Console.Write("Enter product ID to remove (0 to proceed to checkout): ");
                int productId = Helper.GetValidInteger();

                if (productId > 0)
                {
                    ProductService.RemoveFromCart(user, productId);
                }
                else
                {
                    break;
                }
            }

            // Proceed to checkout
            ProductService.Checkout(user);
            Helper.PressKeyToContinue();
        }

        //public static void ShowShippingPage(User user)
        //{
        //    var shippingOptions = new List<string>
        //        {
        //            "1. Standard Shipping - 50 SEK",
        //            "2. Express Shipping - 100 SEK"
        //        };
        //    var windowShipping = new Window("SHIPPING OPTIONS", 2, 2, shippingOptions);
        //    windowShipping.Draw();

        //    Console.SetCursorPosition(2, 6);
        //    Console.Write("Enter shipping option: ");
        //    int shippingOption = Helper.GetValidInteger();
        //    int shippingCost = shippingOption == 1 ? 50 : 100;

        //    Console.Write("Enter your Name: ");
        //    string name = Console.ReadLine();
        //    Console.Write("Enter Address: ");
        //    string address = Console.ReadLine();

        //    user.Cart.ShippingCost = shippingCost;
        //}

        //public static async Task ShowPaymentPage(AppDbContext db, User user)
        //{
        //    var cartItems = user.Cart.Items.Select(i => $"{i.Product.Name} x{i.Quantity} - {i.Product.Price * i.Quantity} SEK").ToList();
        //    cartItems.Add($"Shipping: {user.Cart.ShippingCost} SEK");
        //    cartItems.Add($"Total Price: {user.Cart.Items.Sum(i => i.Product.Price * i.Quantity) + user.Cart.ShippingCost} SEK");

        //    var windowPayment = new Window("PAYMENT", 2, 2, cartItems);
        //    windowPayment.Draw();

        //    var paymentOptions = new List<string> { "1. Credit Card", "2. PayPal" };
        //    var windowPaymentOptions = new Window("CHOOSE PAYMENT METHOD", 2, 8, paymentOptions);
        //    windowPaymentOptions.Draw();

        //    Console.SetCursorPosition(2, 12);
        //    Console.Write("Enter payment method: ");
        //    Console.ReadLine();

        //    await ProductService.Checkout(db, user);
        //    Console.WriteLine("Payment successful! Your order has been placed.");
        //}
    }
}
