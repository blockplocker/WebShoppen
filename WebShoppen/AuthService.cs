using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using WebShoppen.Models;
using Microsoft.EntityFrameworkCore;

namespace WebShoppen
{
    public class AuthService
    {
        public static async Task<User> Login()
        {
            try
            {
                using var db = new AppDbContext();
                Console.Write("Username: ");
                var username = Console.ReadLine();

                Console.Write("Password: ");
                var hashedPassword = HashPassword(Console.ReadLine());

                var user = await db.Users
                    .FirstOrDefaultAsync(u => u.Username == username && u.PasswordHash == hashedPassword);

                if (user == null) Console.WriteLine("Invalid credentials");
                Helper.PressKeyToContinue();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Helper.PressKeyToContinue();
                return null;
            }
        }

        public static async Task<User> Register()
        {
            try
            {
                using var db = new AppDbContext();
                Console.Write("New username: ");
                var username = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(username))
                {
                    Console.WriteLine("Username cannot be empty. Please try again.");
                    Console.Write("New username: ");
                    username = Console.ReadLine();
                }

                Console.Write("New password: ");
                var password = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(password))
                {
                    Console.WriteLine("Password cannot be empty. Please try again.");
                    Console.Write("New password: ");
                    password = Console.ReadLine();
                }
                var hashedPassword = HashPassword(password);

                if (await db.Users.AnyAsync(u => u.Username == username))
                {
                    Console.WriteLine("Username exists");
                    Helper.PressKeyToContinue();
                    return null;
                }

                var user = new User { Username = username, PasswordHash = hashedPassword };
                db.Users.Add(user);
                await db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                Helper.PressKeyToContinue();
                return null;
            }
        }

        public static void ChangePassword(User userToChangePassword)
        {
            using var db = new AppDbContext();
            if (userToChangePassword != null)
            {
                Console.Write("Enter New Password: ");
                var newPassword = Console.ReadLine();
                while (string.IsNullOrWhiteSpace(newPassword))
                {
                    Console.WriteLine("Password cannot be empty. Please try again.");
                    Console.Write("Enter New Password: ");
                    newPassword = Console.ReadLine();
                }
                userToChangePassword.PasswordHash = HashPassword(newPassword);
                db.SaveChanges();
                Console.WriteLine("User password changed successfully.");
            }
            else
            {
                Console.WriteLine("User not found.");
            }
        }
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
