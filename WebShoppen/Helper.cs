using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen
{
    public class Helper
    {
        public static int GetValidInteger()
        {
            while (true)
            {
                try
                {
                    if (int.TryParse(Console.ReadLine(), out int number))
                    {
                        return number;
                    }
                    else
                    {
                        Console.WriteLine("Ogiltigt val, försök igen.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ett fel inträffade: {ex.Message}");
                }
            }
        }

        public static Decimal GetValidDecimal()
        {
            while (true)
            {
                try
                {
                    if (Decimal.TryParse(Console.ReadLine(), out Decimal number))
                    {
                        return number;
                    }
                    else
                    {
                        Console.WriteLine("Wrong input, try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }

        public static void PressKeyToContinue()
        {
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
