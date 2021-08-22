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
            //string startdir = "C:\\";
            //Tree.Print(startdir);
            //do
            //{
            //    Console.Write("Введите директорию ");
            //    string dir = Console.ReadLine();
            //    Tree.Print(dir);
            //} while (true);
            bool flag = false;
            do
            {
                flag = Menu();
            } while (!flag);

            static bool Menu()
            {
                string value = Console.ReadLine();
                string comand = Convert.ToString(value[0]) + Convert.ToString( value[1]);
                if (comand!="ls")
                {
                    Console.WriteLine("Ошибка! Некорректная команда. ");
                    return false;
                }
                Console.WriteLine(comand);
                string userValue = value.Substring(3);
                
                string[] path = userValue.Split(",");
                for (int i = 0; i < path.Length; i++)
                {
                    DirectoryInfo dir = new DirectoryInfo(path[i]);
                    FileInfo file = new FileInfo(path[i]);
                    if (!dir.Exists && !file.Exists)
                    {
                        Console.WriteLine("Ошибка! Некорректный путь.");
                        return false;
                    }
                    path[i] = path[i].Trim();
                    Console.WriteLine(path[i]);
                }
                return true;
            }



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
