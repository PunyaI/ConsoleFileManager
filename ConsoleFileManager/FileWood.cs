using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleFileManager
{
    class Tree
    {
        public static void Print(string dir)
        {
            Console.Clear();
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine();
            Console.WriteLine(dir);
            Console.WriteLine();
            Tree.GetDirsTo2(dir);
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine();
            Tree.GetFilesFromDir(dir);
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine();
        }
        public static void GetFilesFromDir(string dir)
        {
            string[] files = Directory.GetFiles(dir);
            foreach(string file in files)
            {
                FileInfo file_info = new FileInfo(file);
                Console.WriteLine("{0,35}     {1,10}      {2,10} КБ", file_info.Name, file_info.CreationTime, file_info.Length / 1024);
            }
        }
        public static void GetDirsTo2(string start_path)
        {
            try
            {
                string[] dirs = Directory.GetDirectories(start_path);
                foreach (string dir in dirs)
                {
                    DirectoryInfo dir_info = new DirectoryInfo(dir);
                    Console.WriteLine("     ├─ " + dir_info.Name);
                    string[] subdirs = Directory.GetDirectories(dir);
                    foreach (string subdir in subdirs)
                    {
                        DirectoryInfo subdir_info = new DirectoryInfo(subdir);
                        Console.WriteLine("     ├───── " + subdir_info.Name);
                        string[] subdirs2 = Directory.GetDirectories(subdir);
                        foreach (string subdir2 in subdirs2)
                        {
                            DirectoryInfo subdir_info2 = new DirectoryInfo(subdir2);
                            Console.WriteLine("           " + "├───── " + subdir_info2.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
