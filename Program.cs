using System;
using System.IO;

namespace ConsoleFileManager
{
    [Serializable]
    class Program
    {
        static void Main(string[] args)
        {
            FileComands.StartProgram();
            bool flag = false;                  //Выводим наш парсер команд после каждого выполнения, пока пользователь не захочет выйти
            do
            {
                flag = FileComands.Menu();
            } while (!flag);
        }
    }
}
