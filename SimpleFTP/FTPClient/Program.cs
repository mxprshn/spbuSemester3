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
            using (var client = new FileClient(new Client(8888)))
            {
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
                                foreach (var current in await client.List(Console.ReadLine()))
                                {
                                    Console.WriteLine($"{current.Name} {current.IsDirectory}");
                                }
                                break;
                            }
                        case ConsoleKey.D2:
                            {
                                Console.Write("Enter source path: ");
                                var sourcePath = Console.ReadLine();
                                Console.Write("Enter target path: ");
                                var targetPath = Console.ReadLine();
                                await client.Get(sourcePath, targetPath);
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
}