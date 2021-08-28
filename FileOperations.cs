using ConsoleFileManager;
using System;
using System.IO;
using System.Security;

namespace FileManagerConsole
{
    internal class FileOperations
    {
        internal static void CopyDir(DirectoryInfo dir_source, DirectoryInfo dir_target)
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


        internal static void CopyFile(FileInfo file, DirectoryInfo file_target)
        {
            file.CopyTo(file_target.ToString(), true);
            Console.WriteLine($"Файл '{file.Name}' скопирован в каталог '{file_target.Parent}'.");
        }


        internal static void DeleteDir(DirectoryInfo dir)
        {
            try
            {
                dir.Delete(true);
                Console.WriteLine("Каталог успешно удалён");       //удаляем каталог и все вложенные каталоги и файлы рекурсивно, обрабатываем возможные ошибки
            }
            catch (UnauthorizedAccessException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Каталог содержит файл только для чтения");
            }
            catch (DirectoryNotFoundException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Каталог не найден");
            }
            catch (IOException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Каталог доступен только для чтения");
            }
            catch (SecurityException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Недостаточно прав для удаления");
            }

        }


        internal static void DeleteFIle(FileInfo file)
        {
            try
            {                                                                      //удаляем файл и обрабатываем возможные ошибки
                file.Delete();
                Console.WriteLine("Файл успешно удалён");
            }
            catch (SecurityException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Недостаточно прав для удаления");
            }
            catch (IOException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Перед удалением нужно закрыть файл");
            }
        }

        internal static void CreateFile(string file)
        {
            try
            {
                string path = Comands.cur_dir + "\\" + file;
                File.Create(path);
                Console.WriteLine("Файл успешно создан по пути " + path);
            }
            catch (NotSupportedException e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine(e.Message + " Введите только имя файла, например 'mk example.txt'.");
            }
            catch (Exception e)
            {
                ServiceOperations.LogException(e.Message + " " + Comands.cur_dir);
                Console.WriteLine(e.Message);
                Console.WriteLine("Ошибка! Недостаточно прав для создания файла в текущем каталоге:" + Comands.cur_dir);
            }
        }
        internal static void CreateDir(string dir)
        {
            try
            {
                string path = Comands.cur_dir + "\\" + dir;
                Directory.CreateDirectory(path);
                Console.WriteLine("Каталог успешно создан по пути " + path);
            }
            catch(NotSupportedException e)
            {
                ServiceOperations.LogException(e.Message + " " + Comands.cur_dir);
                Console.WriteLine(e.Message + " Введите только имя каталога, например 'mk example'.");
            }
            catch(Exception e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! Недостаточно прав для создания нового каталога в текущем каталоге:" + Comands.cur_dir);
            }
            
        }

        internal static void MoveFile(string file, string target_path)
        {
            try
            {
                string cur_path = Comands.cur_dir + "\\" + file;
                string target = target_path + "\\" + file;
                File.Move(cur_path, target);
                Console.WriteLine($"Файл '{file}' успешно перемещён по пути {target}");
            }
            catch (NotSupportedException e)
            {
                ServiceOperations.LogException(e.Message + " " + Comands.cur_dir);
                Console.WriteLine(e.Message + " Введите только имя файла из текущего каталога и полный путь, куда нужно переместить файл через запятую, например 'mv example.txt, c:\\example\\'.");
            }
            catch (Exception e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! " + e.Message);
            }
        }

        internal static void MoveDir(string dir, string target_path)
        {
            try
            {
                string cur_path = Comands.cur_dir + "\\" + dir;
                string target = target_path + "\\" + dir;
                Directory.Move(cur_path, target);
                Console.WriteLine($"Каталог '{dir}' успешно перемещён по пути {target}");
            }
            catch (NotSupportedException e)
            {
                ServiceOperations.LogException(e.Message + " " + Comands.cur_dir);
                Console.WriteLine(e.Message + " Введите только имя каталога из текущего каталога и полный путь каталога, куда нужно переместить этот каталог через запятую, например 'mv example, c:\\example\\'.");
            }
            catch (Exception e)
            {
                ServiceOperations.LogException(e.Message);
                Console.WriteLine("Ошибка! " + e.Message);
            }
        }

        internal static string UpToRoot(string cur_dir)                              //возвращаемся в коренной каталог
        {
            DirectoryInfo dir = new DirectoryInfo(cur_dir);
            return dir.Root.FullName;
        }


        internal static string Up(string cur_dir)
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(cur_dir);                 //пытаемся вернуть на каталог выше, если не получается, значит мы уже на самом верху
                return dir.Parent.FullName;
            }
            catch
            {
                return cur_dir;
            }

        }
    }
}
