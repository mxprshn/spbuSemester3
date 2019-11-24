using System;
using System.IO;
using System.Threading.Tasks;

namespace FTPClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                using (var client = new FileClient(new Client(8888)))
                {
                    Console.WriteLine("<<< FTP client connected to server\n" +
                        "<<< Command list: \n" +
                        "<<< 1 -- list files in directory\n" +
                        "<<< 2 -- get file from directory\n" +
                        "<<< Esc -- exit\n");

                    while (true)
                    {
                        Console.Write("Enter command: ");

                        var key = Console.ReadKey();

                        Console.WriteLine();

                        try
                        {
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
                        catch (ConnectionToServerException e)
                        {
                            Console.WriteLine($"CONNECTION ERROR: {e.Message}");
                        }
                        catch (ArgumentException e)
                        {
                            Console.WriteLine($"ERROR: {e.Message}");
                        }
                        catch (DirectoryNotFoundException)
                        {
                            Console.WriteLine($"Directory does not exist.");
                        }
                        catch (FileNotFoundException)
                        {
                            Console.WriteLine($"File not found.");
                        }
                    }
                }
            }
            catch (ConnectionToServerException e)
            {
                Console.WriteLine($"CONNECTION ERROR: {e.Message}");
            }            
        }
    }
}