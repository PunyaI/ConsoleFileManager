using ConsoleFileManager;
using System;
using System.IO;

namespace FileManagerConsole
{
    internal class ServiceOperations
    {
        private static string start_dir = "Config\\start_dir.config";

        private static string logs = "Error\\log.txt";
        public static void StartProgram()
        {
            try
            {
                Directory.CreateDirectory("Error");
                Directory.CreateDirectory("Config");
                LogException("Start program." + Environment.NewLine);
                if (ReadStartDir() != null)                  //если конфиг стартовой директории существует и он не пуст - запускаем программу в нашей последней директории
                {
                    PrintUI.PrintTree(ReadStartDir());
                }
            }
          catch
            {
                WriteStartDir(null);
            }
        }


        public static void LogException(string error)
        {
            File.AppendAllText(logs, DateTime.Now + ": " + error);
            File.AppendAllText(logs, Environment.NewLine);
        }

        public static void WriteStartDir(string startdir)          //записываем последнюю директорию в файл для возобновления работы с последнего места
        {
            File.WriteAllText(start_dir, startdir);
        }


        public static string ReadStartDir()
        {
            try                                                  //читаем последнюю директорию из файла, проверяя есть ли вообще этот файл через обработку исключений
            {
                File.ReadAllText(start_dir);
            }
            catch (Exception e)
            {
                LogException(e.Message + " Будет создан новый.");
                return null;
            }
            return File.ReadAllText(start_dir);
        }
    }
}
