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
                    ShowMainMenu(db, ref currentUser);
                }
            }
        }

        static void ShowGuestMenu( ref User currentUser)
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

        static void ShowMainMenu(AppDbContext db, ref User currentUser)
        {
            while (true)
            {
                Console.Clear();
                var userMenu = new List<string>
        {
            $"Logged in as: {currentUser.Username}",
            "1. Homepage",
            "2. Shop",
            "3. View Cart",
            "4. Checkout",
            "5. Admin Panel",
            "6. Logout"
        };
                var windowUserMenu = new Window("MAIN MENU", 2, 2, userMenu);
                windowUserMenu.Draw();

                Console.SetCursorPosition(2, userMenu.Count + 4);
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DisplayShop.ShowHomePage();
                        break;
                    case "2":
                        DisplayShop.ShowShopPage(currentUser);
                        break;
                    case "3":
                        DisplayShop.ShowCartPage(currentUser);
                        break;
                    case "4":
                        ProductService.Checkout(currentUser);
                        break;
                    case "5":
                        Admin.AdminPanel();
                        break;
                    case "6":
                        currentUser = null;
                        return;
                }
            }
        }
    }
}


