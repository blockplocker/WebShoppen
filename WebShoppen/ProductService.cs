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
            user.Cart ??= new Cart { UserId = user.Id };

            var existingItem = user.Cart.Items.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                user.Cart.Items.Add(new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity
                });
            }

            db.SaveChanges();
            Console.WriteLine("Product added to cart!");
        }

        // Remove a product from the cart
        public static void RemoveFromCart(User user, int productId)
        {
            using var db = new AppDbContext();
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

        // Checkout and place an order
        public static void Checkout(User user)
        {
            using var db = new AppDbContext();
            if (user?.Cart == null || !user.Cart.Items.Any())
            {
                Console.WriteLine("Cart is empty");
                return;
            }

            var order = new Order
            {
                UserId = user.Id,
                Items = user.Cart.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList(),
                Total = user.Cart.Items.Sum(i => i.Quantity * i.Product.Price)
            };

            // Remove cart and save order
            db.Carts.Remove(user.Cart);
            db.Orders.Add(order);
            db.SaveChanges();
            Console.WriteLine("Order placed successfully!");
        }
    }
}

