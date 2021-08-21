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
            string startdir = "G:\\Новая папка\\";
            Tree.Print(startdir);
            do
            {
                Console.Write("Введите директорию ");
                string dir = Console.ReadLine();
                Tree.Print(dir);
            } while (true);
            
            



            // FileWood.GetFileWood("G:\\HomeWorkGB\\");


            //string dirName = "G:\\";

            //if (Directory.Exists(dirName))
            //{
            //    Console.WriteLine("Подкаталоги");
            //    string[] dirs = Directory.GetDirectories(dirName);
            //    foreach (string dir in dirs)
            //    {
            //        Console.WriteLine(dir);
            //    }
            //}    
        }
    }
}
