using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pastel;
using System.Drawing;

namespace MyNUnit
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var test in TestRunner.Test(Console.ReadLine()))
            {
                Console.WriteLine($"{test.ClassName} {test.Name} {test.IsPassed} {test.RunTime}".Pastel("#27AE60"));
            }
            Console.ReadKey();
        }
    }
}
