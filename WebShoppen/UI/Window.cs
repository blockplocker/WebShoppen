using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebShoppen
{
    public class Window
    {
        public string Header { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public List<string> TextRows { get; set; }

        public Window(string header, int left, int top, List<string> textRows)
        {
            Header = header;
            Left = left;
            Top = top;
            TextRows = textRows;
        }

        public void Draw()
        {
            var width = TextRows.OrderByDescending(s => s.Length).FirstOrDefault().Length;

            // Kolla om Header är längre än det längsta ordet i listan
            if (width < Header.Length + 4)
            {
                width = Header.Length + 4;
            };

            // Rita Header
            Console.SetCursorPosition(Left, Top);
            if (Header != "")
            {
                Console.Write('┌' + " ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.Write(Header);
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write(" " + new string('─', width - Header.Length) + '┐');
            }
            else
            {
                Console.Write('┌' + new string('─', width + 2) + '┐');
            }

            // Rita raderna i sträng-Listan
            for (int j = 0; j < TextRows.Count; j++)
            {
                Console.SetCursorPosition(Left, Top + j + 1);
                Console.WriteLine('│' + " " + TextRows[j] + new string(' ', width - TextRows[j].Length + 1) + '│');
            }

            // Rita undre delen av fönstret
            Console.SetCursorPosition(Left, Top + TextRows.Count + 1);
            Console.Write('└' + new string('─', width + 2) + '┘');


            // Kolla vilket som är den nedersta posotion, i alla fönster, som ritats ut
            if (Lowest.LowestPosition < Top + TextRows.Count + 2)
            {
                Lowest.LowestPosition = Top + TextRows.Count + 2;
            }

            Console.SetCursorPosition(0, Lowest.LowestPosition);
        }
    }

    public static class Lowest
    {
        public static int LowestPosition { get; set; }
    }
}
