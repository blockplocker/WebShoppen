using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen
{
    public class Helper
    {

        public static int GetValidIntegerMinMax(string prompt, int min = int.MinValue, int max = int.MaxValue)
        {
            int result;
            while (true)
            {
                try
                {
                    Console.Write(prompt);
                    if (int.TryParse(Console.ReadLine(), out result) && result >= min && result <= max)
                        return result;

                    Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
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
                        Console.WriteLine("Wrong input, try again.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
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
