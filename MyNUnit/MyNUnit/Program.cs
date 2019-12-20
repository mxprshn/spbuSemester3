using System;
using Pastel;

namespace MyNUnit
{
    class Program
    {
        public static void Main(string[] args)
        {
            foreach (var test in TestRunner.Test(Console.ReadLine()))
            {
                PrintTestResult(test);
            }

            Console.ReadKey();
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