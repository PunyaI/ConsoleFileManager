using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFileManager
{
    class Program
    {
        static void Main(string[] args)
        {
            bool flag = false;                 ////тест меню
            do
            {
                flag = FileComands.Menu();
            } while (!flag);
        }
    }
}
