using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FileManagerConsole;

namespace ConsoleFileManager
{
    class Comands
    {
        internal static string cur_dir = ServiceOperations.ReadStartDir();


       
        public static bool Menu()                      //метод проверки валидности команд и передачи их в парсеры команд
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
            if (!IsComand(comand))                                                        //проверяем корректность команды
            {
                return false;
            }
            if (value.Length == 2)
            {
                if (ServiceCommand(comand))                                                 //обрабатываем сервисные команды, не требующие пути
                {
                    return false;
                }
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
            if(!FullComandLS(comand,path))                                          //обрабатываем команду ls, если ввели полный путь
            {
                return false;
            }
            ComandFile(comand, path);                                                //обрабатываем все оставшиеся команды работы с файлами
            return false;
        }
        

        private static bool FullComandLS(string comand, string [] path)
        {
            if (comand == "ls" && path[0].Contains(":\\"))                           //обрабатываем команду ls, когда ввели полный путь
            {
                try
                {
                    DirectoryInfo dir_source = new DirectoryInfo(path[0]);
                    if (path.Length > 1 || !dir_source.Exists)
                    {
                        Console.WriteLine("Ошибка! Некорректный путь");
                        return false;
                    }
                    cur_dir = dir_source.FullName;
                    ServiceOperations.WriteStartDir(path[0]);                                 //создаем файл и записываем в него последнюю директорию (если файл есть, то перезаписываем в него новую)
                    PrintUI.PrintTree(path[0]);
                    return false;
                }
                catch (Exception e)
                {
                    ServiceOperations.LogException(e.Message);
                    Console.WriteLine("Ошибка! Некорректный путь");
                    return false;
                }
            }
            return true;
        }

        private static bool IsComand(string comand)
        {
            switch(comand)
            {
                case "ls":
                case "cp":
                case "rm":
                case "fl":
                case "in":
                case "cl":
                case "..":
                case "~~":
                case "mk":
                case "mv":
                    return true;
            }
            Console.WriteLine("Ошибка! Некорректная команда.");
            return false;
        }

        private static bool ServiceCommand(string comand)                                //Парсер сервисных команд, не требующих пути
        {
            switch(comand)
            {
                case "in":
                    PrintUI.PrintReadme();
                    return true;
                case "cl":
                    Console.Clear();
                    return true;
                case "..":
                    cur_dir = FileOperations.Up(cur_dir);
                    ServiceOperations.WriteStartDir(cur_dir);                                 //создаем файл и записываем в него последнюю директорию (если файл есть, то перезаписываем в него новую)
                    PrintUI.PrintTree(cur_dir);
                    return true;
                case "~~":
                    cur_dir = FileOperations.UpToRoot(cur_dir);
                    ServiceOperations.WriteStartDir(cur_dir);                                 //создаем файл и записываем в него последнюю директорию (если файл есть, то перезаписываем в него новую)
                    PrintUI.PrintTree(cur_dir);
                    return true;
                case "ls":
                    if(cur_dir != null)
                    {
                        PrintUI.PrintTree(cur_dir);
                    } else
                    {
                        Console.WriteLine("Ошибка! Текущий путь ещё не задан.");
                    }
                    return true;

            }
            return false;
        }


        private static void ComandFile(string comand, string[] path)                 //парсер команд работы с файловой структурой
        {
            try
            {
                string cur_path = cur_dir + "\\" + path[0];
                FileAttributes source = File.GetAttributes(cur_path);
                switch (comand)
                {
                    case "ls":                                                        //обрабтываем ls, если ввели директорию без полного пути
                        cur_dir = cur_path;
                        ServiceOperations.WriteStartDir(cur_path);                                
                        PrintUI.PrintTree(cur_path);
                        break;
                    case "cp":
                        if (path.Length > 1)                                    //т.к. для копирования нужно 2 пути, проверяем сколько ввели путей
                        {
                            if ((source & FileAttributes.Directory) == FileAttributes.Directory)   //определяем что будем копировать, файл или каталог и вызываем соответствующий метод
                            {
                                FileOperations.CopyDir(path[0], path[1]);
                                Console.WriteLine($"Каталог успешно скопирован.");

                            }
                            else
                            {
                                FileOperations.CopyFile(path[0], path[1]);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Некорректный ввод. Пути каталогов нужно разделять запятой ','");
                            break;
                        };
                        break;
                    case "rm":
                        if (path.Length > 1)
                        {
                            Console.WriteLine("Ошибка! Некорректный путь");
                            break;
                        }
                        if ((source & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            FileOperations.DeleteDir(path[0]);
                        }
                        else
                        {
                            FileOperations.DeleteFIle(path[0]);
                        }

                        break;
                    case "fl":
                        if (path.Length == 1)
                        {
                            PrintUI.PrintFileInfo(path[0]);
                        }
                        else
                        {
                            Console.WriteLine("Ошибка! Некорректный путь");
                            break;
                        }
                        break;
                    case "mv":
                        if (path.Length < 2)
                        {
                            Console.WriteLine("Ошибка! Некорректный путь");
                            break;
                        }
                        if ((source & FileAttributes.Directory) == FileAttributes.Directory)
                        {
                            FileOperations.MoveDir(path[0], path[1]);

                        }
                        else
                        {
                            FileOperations.MoveFile(path[0], path[1]);
                        }
                        break;
                }
            }
            catch(Exception e)
            {
                try
                {
                    if (comand == "mk")                                      //команду создания обрабатываем в блоке catch, если введенного файла или директории не существует 
                    {
                        if (path.Length > 2)
                        {
                            Console.WriteLine("Ошибка! Некорректный путь");
                            return;
                        }
                        if (path[0].Contains("."))
                        {
                            FileOperations.CreateFile(path[0]);
                            return;
                        }
                        else
                        {
                            FileOperations.CreateDir(path[0]);
                            return;
                        }
                    }
                }catch
                {
                    ServiceOperations.LogException(e.Message);
                    Console.WriteLine("Ошибка! " + e.Message);
                }
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! " + e.Message);
            }
            
        }



        


       


    }
}
