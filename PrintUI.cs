using ConsoleFileManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileManagerConsole
{
    internal class PrintUI
    {
        private static int paging = FileManagerConsole.Properties.Settings.Default.Paging;
        private static string tab;
        private static List<string> tree_dir = new List<string>();
        private static List<string> list_files = new List<string>();
        public static void PrintTree(string sdir)                                  //вывод дерева каталогов
        {
            DirectoryInfo dir = new DirectoryInfo(sdir);
            Console.Clear();
            tree_dir.Clear();
            list_files.Clear();
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine("                                       Дерево каталогов ");
            PrintPages(GetDirs(dir, 2)); 
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.Write("                                   Файлы в текущем каталоге ");
            PrintPages(GetFilesFromDir(dir));
            Console.WriteLine();
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine("                                       Инфо о каталоге");
            Console.WriteLine();
            PrintDirInfo(dir);
            Console.WriteLine("_______________________________________________________________________________________________________________________");
        }


        internal static void PrintReadme()
        {
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine("                            Информация о доступных командах");
            Console.WriteLine();
            Console.WriteLine("Для всех команд, где путь указан не полностью (например 'fl source.txt') действия применимы только в текущем каталоге");
            Console.WriteLine("Для выполнения операции в произвольном каталоге, следует сначала перейти в него с помощью команды 'ls (Полный путь)'");
            Console.WriteLine();
            Console.WriteLine("ls ------------------------------- вывод дерева каталогов в текущей директории (если она уже задана)");
            Console.WriteLine("ls Source ------------------------ вывод дерева каталогов");
            Console.WriteLine("ls C:\\Source --------------------- вывод дерева каталогов по полному пути");
            Console.WriteLine("mk Source ------------------------ создание нового каталога 'Source' в текущем каталоге");
            Console.WriteLine("mk source.txt -------------------- создание нового файла 'source.txt' в текущем каталоге");
            Console.WriteLine("cp Source, D:\\Target ------------- копирование каталога (со всем содержимым)");
            Console.WriteLine("cp source.txt, D:\\target.txt ----- копирование файла");
            Console.WriteLine("rm Source ------------------------ удаление каталога (со всем содержимым)");
            Console.WriteLine("rm source.txt -------------------- удаление файла");
            Console.WriteLine("fl source.txt -------------------- вывод информации о файле");
            Console.WriteLine("in ------------------------------- вывод списка команд");
            Console.WriteLine("ex ------------------------------- выход из программы");
            Console.WriteLine("cl ------------------------------- очистка консоли");
            Console.WriteLine(".. ------------------------------- переход на уровень вверх");
            Console.WriteLine("~~ ------------------------------- переход в корневой каталог");
            Console.WriteLine("_______________________________________________________________________________________________________________________");
        }

        private static long GetDirectorySize(string sdir)            //находим количество всех каталогов в директории для последующего разбиения на страницы
        {
            DirectoryInfo dir = new DirectoryInfo(sdir);
            return dir.EnumerateFiles("*.*", SearchOption.AllDirectories).Sum(file => file.Length);
        }


        private static List<string> GetFilesFromDir(DirectoryInfo dir)        //вывод общего списка файлов и их свойств в текущем каталоге 
        {
            foreach (FileInfo file in dir.GetFiles())
            {
                list_files.Add(String.Format("{0,35}     {1,15}      {2,15} КБ", file.Name, file.CreationTime, file.Length / 1024));
            }
            return list_files;
        }


        private static List<string> GetDirs(DirectoryInfo startdir, int level)              //level - глубина рекурсии
        {
            level--;
            if (Directory.Exists(startdir.FullName))
            {
                try
                {
                    foreach (DirectoryInfo dir in startdir.GetDirectories())               //обходим все директории
                    {
                        if (Directory.Exists(dir.FullName))
                        {
                            switch (level)                                                 //добавляем табуляцию для разных уровней вложенности
                            {
                                case 0:
                                    tab = "   │       └───";
                                    break;
                                case 1:
                                    tab = "   ├──";
                                    break;
                            }
                            tree_dir.Add(tab + dir.Name);                                        //обавляем табулированную строку с каталогом в коллекцию
                            if (level > 0)                                                   //и на заданную глубину обходим все поддиректории
                            {
                                GetDirs(dir, level);
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    ServiceOperations.LogException(e.Message);
                }
            }
            return tree_dir;
        }


        private static void PrintPages(List<string> tree)
        {
            if (tree.Count == 0 && tree_dir.Count == 0)
            {
                return;
            }
            int allpages = tree.Count / paging + 1;
            Console.WriteLine(/*"Текущий каталог: " + */Comands.cur_dir);
            Console.WriteLine();
            Page(1, tree);                      //Всегда сначала выводим 1 страницу и спрашиваем что делать дальше, показать другие страницы, или перейти к командам
            if (allpages <= 1)                   //если страница всего одна, то выходим без дальнейшего выбора страниц
            {
                return;
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine($"Вы на странице 1 (вывод по {paging} элементов).");
                Console.WriteLine("Нажмите 'Enter' и введите номер страницы, на которую хотите перейти или 'Esc' для продолжения работы.");
            }
            while (Console.ReadKey().Key !=ConsoleKey.Escape)                                    //цикл вывода страниц по запросу их порядковых номеров, пока не будет введена команда skip для продолжения работы
            {
                string input = Console.ReadLine();
                if (Int32.TryParse(input, out int res) && res <= allpages)
                {
                    Page(res, tree);
                    Console.WriteLine($"Вы на странице {res} (вывод по {paging} элементов).");
                    Console.WriteLine("Нажмите 'Enter' и введите номер страницы, на которую хотите перейти или 'Esc' для продолжения работы.");
                }
                else
                {
                    Console.WriteLine("Ошибка! Страница не найдена.");
                    Console.WriteLine("Нажмите 'Enter' и введите номер страницы, на которую хотите перейти или 'Esc' для продолжения работы.");
                }

            }
        }


        private static void Page(int num_page, List<string> tree)
        {
            for (int j = (num_page - 1) * paging; j < paging * num_page; j++)          //выводим запрошенную страницу списка
            {
                if (j < tree.Count)
                {
                    Console.WriteLine(tree[j]);
                }
                else
                {
                    return;
                }
            }
        }


        internal static void PrintFileInfo(string filename)
        {
                string path = Comands.cur_dir + "\\" + filename;
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    Console.WriteLine("_______________________________________________________________________________________________________________________");
                    Console.WriteLine();
                    Console.WriteLine("Имя файла:                 " + file.Name);
                    Console.WriteLine("Расширение файла:          " + file.Extension);
                    Console.WriteLine("Размер файла:              " + file.Length / 1024 / 1024 + " МБ");
                    Console.WriteLine("Создан:                    " + file.CreationTime);
                    Console.WriteLine("Последнее изменение:       " + file.LastWriteTime);
                    Console.WriteLine("Только для чтения:         " + file.IsReadOnly);
                    Console.WriteLine("Корневой каталог файла:    " + file.DirectoryName);
                    Console.WriteLine("_______________________________________________________________________________________________________________________");
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine($"Ошибка! Файл '{file.FullName}' не найден.");
                }
        }


        private static void PrintDirInfo(DirectoryInfo dir)                                   //выводим инфо о каталоге по аналогии с файлом
        {
            Console.WriteLine("Имя каталога:        " + dir.Name);
            Console.WriteLine("Корневой каталог:    " + dir.Parent);
            Console.WriteLine("Создан:              " + dir.CreationTime);
            Console.WriteLine("Последнее изменение: " + dir.LastWriteTime);
            try
            {
                long size = GetDirectorySize(dir.FullName.ToString()) / 1024 / 1024;
                Console.WriteLine("Размер каталога:     " + size + " МБ");
            }
            catch (Exception e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Размер каталога:       Не удалось измерить. Недостаточно прав.");
            }

        }
    }
}
