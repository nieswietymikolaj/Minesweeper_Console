using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Saper
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.Unicode;
            Console.CursorVisible = false;
            Console.SetWindowSize(80, 30);
            Console.SetBufferSize(80, 30);
            Console.Title = " SAPER";

            Menu menu = new Menu();

            while (true)
            {
                if (menu.MainMenu() == -1)
                {
                    break;
                }
            }
        }
    }
}
