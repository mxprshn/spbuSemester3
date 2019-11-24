using System;

namespace FTPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new Server(8888, new FileQueryParser());
            server.Run();

            Console.WriteLine("<<< Server launched.\n" +
                "<<< Press 'Esc' to exit.\n");

            while (true)
            {
                var key = Console.ReadKey();

                if (key.Key == ConsoleKey.Escape)
                {
                    server.Shutdown();
                    return;
                }
            }
        }
    }
}