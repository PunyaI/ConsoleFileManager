using System;
using System.IO;

namespace ConsoleFileManager
{
    [Serializable]
    class Program
    {
        static void Main(string[] args)
        {
            Directory.CreateDirectory("Error");
            Directory.CreateDirectory("Config");
            FileComands.LogException("Start program." + Environment.NewLine);
            if (FileComands.ReadStartDir() != null)                  //если конфиг стартовой директории существует и он не пуст - запускаем программу в нашей последней директории
            {
                FileComands.PrintTree(FileComands.ReadStartDir());
            }
            bool flag = false;                  //Выводим наш парсер команд после каждого выполнения, пока пользователь не захочет выйти
            do
            {
                flag = FileComands.Menu();
            } while (!flag);
        }
    }
}
