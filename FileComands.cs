using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;


namespace ConsoleFileManager
{
    [Serializable]
    class FileComands
    {
        private static string logs = "Error\\log.txt";
        private static string start_dir = "Config\\start_dir.config";
        private static string tab;
        private static int paging = FileManagerConsole.Properties.Settings.Default.Paging;
        private static List<string> tree_dir = new List<string>();
        private static List<string> list_files = new List<string>();
        private static string cur_dir;

        public static bool Menu()
        {
            Console.WriteLine("Введите команду. Для вызова списка команд введите 'in'. Для выхода из программы введите 'ex'.");
            string value = Console.ReadLine();
            if (value.Length < 2)                                                           //ловим ситуацию, когда введено меньше 2 символов
            {
                Console.WriteLine("Ошибка! Некорректная команда.");
                return false;
            }
            string comand = Convert.ToString(value[0]) + Convert.ToString(value[1]);      //вытаскиваем команду из введенной строки
            if (comand == "ex")                                                           //сразу обрабатываем команду выхода из программы
            {
                return true;
            }
            if (!IsComand(comand))                                                        //проверяем корректность команды и внутри обрабатываем команду "инфо"
            {
                return false;
            }
            if (value.Length <= 3)                                                         //проверяем есть ли путь для оставшихся команд
            {
                Console.WriteLine("Ошибка! Некорректная команда.");
                return false;
            }
            string userValue = value.Substring(3);                                    //обрезаем входную строку от команды, чтобы оставить только пути
            string[] path = userValue.Split(',');                                     //выделяем пути, разделенные запятой
            if (path.Length > 2)                                                      //обрабатываем ситуацию с 3 и более путями в команде
            {
                Console.WriteLine("Ошибка! Некорректная команда.");
                return false;
            }
            else
            {
                for (int i = 0; i < path.Length; i++)                                  //обрезаем лишние пробелы в начале и в конце у путей
                {
                    path[i] = path[i].Trim();
                }
            }
            DirectoryInfo dir_source = new DirectoryInfo(path[0]);
            FileInfo file_source = new FileInfo(path[0]);

            switch (comand)                                                             //парсер команд
            {
                case "ls":
                    if (path.Length > 1 || !dir_source.Exists)                   //обрабатываем индивидуальные ошибки ввода пути для каждой команды
                    {
                        Console.WriteLine("Ошибка! Некорректный путь");
                        return false;
                    }
                    cur_dir = dir_source.FullName;
                    WriteStartDir(path[0]);                                 //создаем файл и записываем в него последнюю директорию (если файл есть, то перезаписываем в него новую)
                    PrintTree(path[0]);
                    break;
                case "cp":
                    if (path.Length > 1)                                    //т.к. для копирования нужно 2 пути, проверяем сколько ввели путей
                    {

                        DirectoryInfo file_target = new DirectoryInfo(path[1]);
                        DirectoryInfo dir_target = new DirectoryInfo(path[1]);
                        if (file_source.Exists)                                                  //определяем что будем копировать, файл или каталог и вызываем соответствующий метод
                        {
                            CopyFile(file_source, file_target);
                        }
                        else
                        if (dir_source.Exists)
                        {

                            CopyDir(dir_source, dir_target);
                            Console.WriteLine($"Каталог успешно скопирован.");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Некорректный путь");
                            return false;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Ошибка! Некорректный ввод. Пути каталогов нужно разделять запятой ','");
                        return false;
                    };
                    break;
                case "rm":
                    if (file_source.Exists)
                    {
                        DeleteFIle(file_source);
                    }
                    else
                    if (dir_source.Exists)
                    {
                        DeleteDir(dir_source);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка! Некорректный путь");
                        return false;
                    }
                    break;
                case "fl":
                    if (file_source.Exists)
                    {
                        PrintFileInfo(file_source);
                    }
                    else
                    {
                        Console.WriteLine("Ошибка! Некорректный путь");
                        return false;
                    }
                    break;
            }
            return false;
        }

        public static void LogException(string error)
        {
            File.AppendAllText(logs, DateTime.Now + ": " + error);
            File.AppendAllText(logs, Environment.NewLine);
        }
        private static bool IsComand(string comand)
        {
            if (comand != "ls" && comand != "cp" && comand != "rm" && comand != "fl" && comand != "in" && comand != "cl")   //проверяем является ли строка командой и обрабатываем некоторые из них
            {
                Console.WriteLine("Ошибка! Некорректная команда.");
                return false;
            }
            else if (comand == "in")
            {
                Console.WriteLine("ls C:\\Source                        - вывод дерева файловой системы");
                Console.WriteLine("cp C:\\Source, D:\\Target             - копирование каталога (со всем содержимым)");
                Console.WriteLine("cp C:\\source.txt, D:\\target.txt     - копирование файла");
                Console.WriteLine("rm C:\\Source                        - удаление каталога (со всем содержимым)");
                Console.WriteLine("rm C:\\source.txt                    - удаление файла");
                Console.WriteLine("fl C:\\source.txt                    - вывод содержимого файла");
                Console.WriteLine("in                                   - вывод списка команд");
                Console.WriteLine("ex                                    - выход из программы");
                Console.WriteLine("cl                                    - очистка консоли");
                return false;
            }
            else if (comand == "cl")
            {
                Console.Clear();
                return false;
            }
            return true;
        }

        public static void WriteStartDir(string startdir)          //записываем последнюю директорию в файл для возобновления работы с последнего места
        {
            File.WriteAllText(start_dir,startdir);
        }


        public static string ReadStartDir()
        {
            try                                                  //читаем последнюю директорию из файла, проверяя есть ли вообще этот файл через обработку исключений
            {
               File.ReadAllText(start_dir);
            }
            catch(Exception e)
            {
                LogException(e.Message + " Будет создан новый.");
                return null;
            }
            return File.ReadAllText(start_dir);
        }

        public static void PrintTree(string sdir)                                  //вывод дерева каталогов
        {
            DirectoryInfo dir = new DirectoryInfo(sdir);
            Console.Clear();
            tree_dir.Clear();
            list_files.Clear();
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            PrintPages(GetDirs(dir, 3));
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine("                                   Файлы в текущем каталоге");
            Console.WriteLine();
            PrintPages(GetFilesFromDir(dir));
            Console.WriteLine("_______________________________________________________________________________________________________________________");
            Console.WriteLine("                                        Инфо о каталоге");
            Console.WriteLine();
            PrintDirInfo(dir);
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
                                    tab = "                └─── ";
                                    break;
                                case 1:
                                    tab = "        └─── ";
                                    break;
                                case 2:
                                    tab = "   └─ ";
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
                    LogException(e.Message);
                }
            }
            return tree_dir;
        }


        private static void PrintPages(List<string> tree)
        {
            int allpages = tree.Count / paging+1;
            if (tree[0] == tree_dir[0])                          //если выводим дерево каталогов - выводим на консоль также корневой каталог
            {
                Console.WriteLine("Текущий каталог: " + cur_dir);
                Console.WriteLine(" " + cur_dir);
            }
            Page(1, tree);                      //Всегда сначала выводим 1 страницу и спрашиваем что делать дальше, показать другие страницы, или перейти к командам
            if (allpages == 1)                   //если страница всего одна, то выходим без дальнейшего выбора страниц
            {
                return;
            } else
            {
                Console.WriteLine("Вы на странице 1. Введите номер страницы, на которую хотите перейти или skip для продолжения работы.");
            }
            while(true)                                    //цикл вывода страниц по запросу их порядковых номеров, пока не будет введена команда skip для продолжения работы
            {
                string input = Console.ReadLine();
                if (Int32.TryParse(input, out int res) && res <= allpages)
                {
                    Page(res,tree);
                    Console.WriteLine($"Вы на странице {res}. Введите номер страницы, на которую хотите перейти или skip для продолжения работы.");
                }
                else if (input == "skip")
                {
                    return;
                }
                else
                {
                    Console.WriteLine("Ошибка! Такой страницы нет. Введите номер страницы, на которую хотите перейти или skip для продолжения работы.");
                }

            } 
        }


        private static void Page(int num_page, List<string> tree)
        {
            for (int j = (num_page - 1) * paging; j < paging * num_page; j++)          //выводим запрошенную страницу списка
            {
                if(j<tree.Count)
                {
                    Console.WriteLine(tree[j]);
                }
            }
        }



        private static void CopyDir(DirectoryInfo dir_source, DirectoryInfo dir_target)
        {
            if (dir_source.FullName.ToLower() == dir_target.FullName.ToLower())               //если исходная и целевая директория совпадают - делаем копию директории
            {
                DirectoryInfo dir_target_copy = new DirectoryInfo(dir_target.FullName.ToString() + "-copy");
                dir_target = dir_target_copy;
                Console.WriteLine("Каталоги совпадают, будет создан каталог " + dir_target_copy.Name);
            }
            if (!Directory.Exists(dir_target.FullName))                                     //если целевой директории не существует - создаем её
            {
                Directory.CreateDirectory(dir_target.FullName);
            }
            foreach (FileInfo file in dir_source.GetFiles())                                 //копируем всё файлы из текущей директории в целевую
            {
                file.CopyTo(Path.Combine(dir_target.ToString(), file.Name), true);
            }
            foreach (DirectoryInfo source_subdir in dir_source.GetDirectories())            //рекурсивно копируем все вложенные директории и файлы в них
            {
                DirectoryInfo next_target_subdir = dir_target.CreateSubdirectory(source_subdir.Name);
                CopyDir(source_subdir, next_target_subdir);
            }

        }


        private static void CopyFile(FileInfo file, DirectoryInfo file_target)
        {
            file.CopyTo(file_target.ToString(), true);
            Console.WriteLine($"Файл '{file.Name}' скопирован в каталог '{file_target.Parent}'.");
        }


        private static void DeleteDir(DirectoryInfo dir)
        {
            try
            {
                dir.Delete(true);
                Console.WriteLine("Каталог успешно удалён");       //удаляем каталог и все вложенные каталоги и файлы рекурсивно, обрабатываем возможные ошибки
            }
            catch (UnauthorizedAccessException e)
            {
                LogException(e.Message);
                Console.WriteLine("Ошибка! Каталог содержит файл только для чтения");
            }
            catch (DirectoryNotFoundException e)
            {
                LogException(e.Message);
                Console.WriteLine("Ошибка! Каталог не найден");
            }
            catch (IOException e)
            {
                LogException(e.Message);
                Console.WriteLine("Ошибка! Каталог доступен только для чтения");
            }
            catch (SecurityException e)
            {
                LogException(e.Message);
                Console.WriteLine("Ошибка! Недостаточно прав для удаления");
            }

        }


        private static void DeleteFIle(FileInfo file)
        {
                try
                {                                                                      //удаляем файл и обрабатываем возможные ошибки
                    file.Delete();
                    Console.WriteLine("Файл успешно удалён");
                }
                catch (SecurityException e)
                {
                    LogException(e.Message);
                    Console.WriteLine("Ошибка! Недостаточно прав для удаления");
                }
                catch (IOException e)
                {
                    LogException(e.Message);
                    Console.WriteLine("Ошибка! Перед удалением нужно закрыть файл");
                }
        }


        private static void PrintFileInfo(FileInfo file)
        {
                try                           //вывыодим подробное инфо о запрошенном файле, обрабатывая исключение с ограничением доступа
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
                catch(Exception e)
                {
                    LogException(e.Message);
                    Console.WriteLine("Ошибка! Недостаточно прав");
                }
        }


        public static void PrintDirInfo(DirectoryInfo dir)                                   //выводим инфо о каталоге по аналогии с файлом
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
            catch(Exception e)
            {
                LogException(e.Message);
                Console.WriteLine("Размер каталога:       Не удалось измерить. Недостаточно прав.");
            }

        }
    }
}
