using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FTPClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new FileClient(new Client(8888));

            while (true)
            {
                Console.Write("Enter command: ");

                var key = Console.ReadKey();

                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.D1:
                        {
                            Console.Write("Enter directory path to list files: ");
                            await client.ListFiles(Console.ReadLine());
                            break;
                        }
                    case ConsoleKey.D2:
                        {
                            Console.Write("Enter file path: ");
                            await client.GetFile(Console.ReadLine());
                            break;
                        }
                    case ConsoleKey.Escape:
                        {
                            return;
                        }
                }
            }
        }
    }
}