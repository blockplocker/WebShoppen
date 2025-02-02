using Microsoft.EntityFrameworkCore;
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
        private static void ManageUsers()
        {
            using var db = new AppDbContext();
            while (true)
            {
                Console.Clear();
                var userOptions = new List<string>
        {
            "1. Edit Customer Details",
            "2. View Order History",
            "3. Delete User",
            "4. Change User Admin Status",
            "5. Change User Password",
            "6. Back to Admin Panel"
        };

                var windowUsers = new Window("MANAGE USERS", 2, 2, userOptions);
                windowUsers.Draw();

                var users = db.Users.ToList();
                var userLines = users.Select(u => $"Id: {u.Id} {u.Username} {(u.IsAdmin ? "Admin:" : "User:")} ").ToList();

                new Window("USER LIST", 40, 2, userLines).Draw();

                Console.SetCursorPosition(2, userOptions.Count + 4);
                int choice = Helper.GetValidIntegerMinMax("Select option: ", 1, 6);

                switch (choice)
                {
                    case 1: // Edit Customer Details
                        EditCustomerDetails();
                        break;

                    case 2: // View Order History
                        ViewOrderHistory();
                        break;

                    case 3: // Delete User
                        DeleteUser();
                        break;

                    case 4: // Change Admin Status
                        ToggleAdminStatus();
                        break;

                    case 5: // Change Password
                        ChangeUserPassword();
                        break;

                    case 6: // Exit
                        return;
                }
                Helper.PressKeyToContinue();
            }
        }

        private static void EditCustomerDetails()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();

            var user = db.Users
                .Include(u => u.Customer)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                Console.WriteLine("User not found!");
                return;
            }

            if (user.Customer == null)
            {
                Console.WriteLine("No customer details exist. Create new? (Y/N)");
                if (Console.ReadKey().Key != ConsoleKey.Y)
                    return;

                user.Customer = new Customer { UserId = user.Id };
            }

            EditCustomerField("Full Name", user.Customer.FullName, v => user.Customer.FullName = v);
            EditCustomerField("Address", user.Customer.Address, v => user.Customer.Address = v);
            EditCustomerField("City", user.Customer.City, v => user.Customer.City = v);
            EditCustomerField("Postal Code", user.Customer.PostalCode, v => user.Customer.PostalCode = v);
            EditCustomerField("Country", user.Customer.Country, v => user.Customer.Country = v);
            EditCustomerField("Phone", user.Customer.Phone, v => user.Customer.Phone = v);

            db.SaveChanges();
            Console.WriteLine("Customer details updated!");
        }

        private static void EditCustomerField(string fieldName, string currentValue, Action<string> setter)
        {
            Console.Write($"{fieldName} ({currentValue}): ");
            var newValue = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(newValue))
            {
                setter(newValue);
            }
        }

        private static void ViewOrderHistory()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();

            var customer = db.Customers
                .Include(c => c.User)
                .FirstOrDefault(c => c.UserId == userId);

            if (customer == null)
            {
                Console.WriteLine("No customer details found!");
                return;
            }

            var orders = db.Orders
                .Where(o => o.CustomerId == customer.Id)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            var orderLines = new List<string>();
            foreach (var order in orders)
            {
                orderLines.Add($"Order #{order.Id} - {order.OrderDate:yyyy-MM-dd}");
                orderLines.Add($"Total: {order.Total + order.ShippingCost + order.VAT:C}");
                orderLines.AddRange(order.Items.Select(i =>
                    $"{i.Product.Name} x{i.Quantity} @ {i.Product.Price:C}"
                ));
                orderLines.Add("");
            }

            new Window($"ORDER HISTORY FOR {customer.FullName}", 2, 2, orderLines).Draw();
        }

        private static void DeleteUser()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID to Delete: ");
            int userId = Helper.GetValidInteger();
            var user = db.Users.Find(userId);

            if (user != null)
            {
                MongoLogger.Log("UserDeleted", $"Deleted user: {user.Username}");

                db.Users.Remove(user);
                db.SaveChanges();
                Console.WriteLine("User deleted successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private static void ToggleAdminStatus()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();
            var user = db.Users.Find(userId);

            if (user != null)
            {
                user.IsAdmin = !user.IsAdmin;
                db.SaveChanges();
                Console.WriteLine($"Admin status changed to: {user.IsAdmin}");

                MongoLogger.Log("AdminStatusChanged", $"User {user.Username} status: {user.IsAdmin}");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }

        private static void ChangeUserPassword()
        {
            using var db = new AppDbContext();
            Console.Write("Enter User ID: ");
            int userId = Helper.GetValidInteger();
            var user = db.Users.Find(userId);

            AuthService.ChangePassword(user);
        }
    }
}
