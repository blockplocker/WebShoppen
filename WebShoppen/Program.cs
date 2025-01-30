using Microsoft.EntityFrameworkCore;
using WebShoppen.Models;


namespace WebShoppen
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using var db = new AppDbContext();
            db.Database.EnsureCreated();

            User currentUser = null;

            while (true)
            {
                Console.Clear();

                if (currentUser == null)
                {
                    ShowGuestMenu(ref currentUser);
                }
                else
                {
                    currentUser = ShowMainMenu(currentUser);
                }
            }
        }

        static void ShowGuestMenu(ref User currentUser)
        {
            var guestMenu = new List<string> { "1. Login", "2. Register", "3. Exit" };
            var windowGuest = new Window("WELCOME TO WEBSHOP", 2, 2, guestMenu);
            windowGuest.Draw();

            Console.SetCursorPosition(2, guestMenu.Count + 4);
            var choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    currentUser = AuthService.Login().GetAwaiter().GetResult();
                    break;
                case "2":
                    currentUser = AuthService.Register().GetAwaiter().GetResult();
                    break;
                case "3":
                    Environment.Exit(0);
                    break;
            }
        }

        static User ShowMainMenu(User currentUser)
        {

            while (true)
            {
            using var db = new AppDbContext();
                // Ensure the user's cart is loaded from the database
                currentUser.Cart = db.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product) // Ensure products are loaded
                    .FirstOrDefault(c => c.UserId == currentUser.Id);

                Console.Clear();
                var userMenu = new List<string>
        {
            $"Logged in as: {currentUser.Username}",
            "1. Shop",
            "2. View Cart",
            "3. Logout",
            (currentUser.IsAdmin ? "5. Admin Panel" : "")
        };
                var windowUserMenu = new Window("MAIN MENU", 2, 2, userMenu);
                windowUserMenu.Draw();

                var welcomeText = new List<string> { "Welcome to Our Webshop!", "Find the best products at great prices!" };
                var windowWelcome = new Window("HOME", 30, 2, welcomeText);
                windowWelcome.Draw();

                var featuredProducts = db.Products.Where(p => p.IsFeatured).Take(3).ToList();

                if (featuredProducts.Count > 0)
                {
                    var Product1 = featuredProducts[0];
                    var featuredProductList1 = new List<string> { Product1.Name, Product1.Description, "Price: " + Product1.Price + " kr", "Press A to buy" };
                    var windowFeatured1 = new Window("FEATURED PRODUCT 1", 2, 11, featuredProductList1);
                    windowFeatured1.Draw();
                }

                if (featuredProducts.Count > 1)
                {
                    var Product2 = featuredProducts[1];
                    var featuredProductList2 = new List<string> { Product2.Name, Product2.Description, "Price: " + Product2.Price + " kr", "Press B to buy" };
                    var windowFeatured2 = new Window("FEATURED PRODUCT 2", 35, 11, featuredProductList2);
                    windowFeatured2.Draw();
                }

                if (featuredProducts.Count > 2)
                {
                    var Product3 = featuredProducts[2];
                    var featuredProductList3 = new List<string> { Product3.Name, Product3.Description, "Price: " + Product3.Price + " kr", "Press C to buy" };
                    var windowFeatured3 = new Window("FEATURED PRODUCT 3", 68, 11, featuredProductList3);
                    windowFeatured3.Draw();
                }

                if (currentUser?.Cart?.Items == null || !currentUser.Cart.Items.Any())
                {
                    var emptyCart = new List<string> { "Your cart is empty!" };
                    var windowCart = new Window("CART", 120, 2, emptyCart);
                    windowCart.Draw();
                }
                else
                {
                    var cartItems = currentUser.Cart.Items.Select(i => $"{i.Product.Name} x{i.Quantity} ").ToList();
                    var windowCartWithItems = new Window("CART", 100, 2, cartItems);
                    windowCartWithItems.Draw();
                }

                Console.SetCursorPosition(2, 18);
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "A":
                    case "a":
                        if (featuredProducts.Count > 0)
                        {
                            ProductService.AddToCart(currentUser, featuredProducts[0].Id, 1);
                        }
                        break;
                    case "B":
                    case "b":
                        if (featuredProducts.Count > 1)
                        {
                            ProductService.AddToCart(currentUser, featuredProducts[1].Id, 1);
                        }
                        break;
                    case "C":
                    case "c":
                        if (featuredProducts.Count > 2)
                        {
                            ProductService.AddToCart(currentUser, featuredProducts[2].Id, 1);
                        }
                        break;
                    case "1":
                        DisplayShop.ShowShopPage(currentUser);
                        break;
                    case "2":
                        DisplayShop.ShowCartPage(currentUser);
                        break;
                    case "3":
                        return null;
                    case "5":
                        if (currentUser.IsAdmin)
                        {
                            Admin.AdminPanel();
                        }
                        break;
                }
            }
        }
    }
}


