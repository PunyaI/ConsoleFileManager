using FileManagerConsole;
using System;

namespace ConsoleFileManager
{
    [Serializable]
    class Program
    {
        static void Main(string[] args)
        {
            ServiceOperations.StartProgram();
            bool flag = false;                  //Выводим наш парсер команд после каждого выполнения, пока пользователь не захочет выйти
            do
            {
                flag = Comands.Menu();
            } while (!flag);
        }
    }
}
