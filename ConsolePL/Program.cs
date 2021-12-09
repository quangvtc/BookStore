using System;
using System.IO;

namespace ConsolePL
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            UserInterFace UI = new UserInterFace();
            UI.MainUI();
        }
    }
}
