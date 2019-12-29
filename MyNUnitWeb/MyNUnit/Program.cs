using System;
using System.IO;
using Pastel;

namespace MyNUnit
{
    class Program
    {
        public static void Main(string[] args)
        {
            while (true)
            {
                Console.WriteLine("* MyNUnit Test Runner * ");
                Console.WriteLine("* Enter 'bye' to exit * ");
                Console.Write("* Enter path to the folder with tests: ");

                var input = Console.ReadLine();

                if (input == "bye")
                {
                    return;
                }

                try
                {
                    var results = TestRunner.Test(input);

                    if (results.Count == 0)
                    {
                        Console.WriteLine($"Test not found.");
                    }

                    foreach (var result in results)
                    {
                        PrintTestResult(result);
                    }
                }
                catch (TestRunnerException exception)
                {
                    Console.WriteLine($"Test Runner error: {exception.Message}");
                }
                catch (DirectoryNotFoundException)
                {
                    Console.WriteLine($"Directory not found.");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Assemblies not found.");
                }
            }

        }

        private static void PrintTestResult(ITest test)
        {
            if (test.IsPassed == true)
            {
                Console.WriteLine($"{test.Name} in class {test.ClassName} passed in {test.RunTime}.".Pastel("#27AE60"));
            }
            else if (test.IsPassed == false)
            {
                Console.WriteLine($"{test.Name} in class {test.ClassName} failed.".Pastel("#E74C3C"));
            }
            else
            {
                Console.WriteLine($"{test.Name} in class {test.ClassName} is ignored: {test.IgnoreReason}.");
            }
        }
    }
}