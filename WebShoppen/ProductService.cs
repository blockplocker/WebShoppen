using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebShoppen.Models;

namespace WebShoppen
{
    internal class ProductService
    {
        // Add a product to the cart
        public static void AddToCart(User user, int productId, int quantity)
        {
            using var db = new AppDbContext();

            // Ensure the user's cart is loaded from the database
            user.Cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) // Ensure products are loaded
                .FirstOrDefault(c => c.UserId == user.Id) ?? new Cart { UserId = user.Id, Items = new List<CartItem>() };

            // Ensure the cart is attached to the context
            if (user.Cart.Id == 0)
            {
                db.Carts.Add(user.Cart);
            }
            else
            {
                db.Carts.Attach(user.Cart);
            }

            var existingItem = user.Cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                var product = db.Products.Find(productId);
                if (product == null)
                {
                    Console.WriteLine("Product not found.");
                    return;
                }

                user.Cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    Product = product // Attach the product to the cart item
                });
            }

            db.SaveChanges();
            Console.WriteLine("Product added to cart!");
        }

        public static void RemoveFromCart(User user, int productId)
        {
            using var db = new AppDbContext();

            // Ensure the user's cart is loaded from the database
            user.Cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product) // Ensure products are loaded
                .FirstOrDefault(c => c.UserId == user.Id);

            if (user?.Cart == null || !user.Cart.Items.Any())
            {
                Console.WriteLine("Cart is empty");
                return;
            }

            var itemToRemove = user.Cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (itemToRemove != null)
            {
                user.Cart.Items.Remove(itemToRemove);
                db.SaveChanges();
                Console.WriteLine("Item removed from cart!");
            }
            else
            {
                Console.WriteLine("Item not found in cart.");
            }
        }

        public static void Checkout(User user, int shippingCost)
        {
            using var db = new AppDbContext();

            // Ensure all relationships are loaded
            user.Cart = db.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefault(c => c.UserId == user.Id);

            // Load customer
            var customer = db.Customers.FirstOrDefault(c => c.UserId == user.Id);
            if (customer == null)
            {
                Console.WriteLine("Customer details not found. Complete shipping first.");
                Helper.PressKeyToContinue();
                return;
            }

            // Calculate totals
            var total = user.Cart.Items.Sum(i => i.Quantity * i.Product.Price);
            var vat = (total + shippingCost) * 0.25m;

            // Create order
            var order = new Order
            {
                UserId = user.Id,
                CustomerId = customer.Id,
                OrderDate = DateTime.UtcNow,
                Items = user.Cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),
                Total = total,
                ShippingCost = shippingCost,
                VAT = vat
            };

            // Save and clear cart
            db.Orders.Add(order);
            db.Carts.Remove(user.Cart);
            db.SaveChanges();

            MongoLogger.Log("OrderPlaced", $"User {user.Username} placed order");

            Console.WriteLine("Payment successful! Your order has been placed.");
            Helper.PressKeyToContinue();
        }
    }
}

