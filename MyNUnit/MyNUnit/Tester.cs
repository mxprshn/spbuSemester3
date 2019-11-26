using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public static class Tester
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Test(string path)
        {
            // проверить путь

            logger.Info("ololo.");
            var executableNames = Directory.EnumerateFiles(path, "*.exe; *.dll", SearchOption.AllDirectories);

            foreach (var name in executableNames)
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(name);
                    var assembly = Assembly.Load(assemblyName);
                }
                catch (Exception e)
                {
                    logger.Error(e, "Invalid executable file.");
                }
            }
        }

        private static void TestAssembly(Assembly assembly)
        {
            foreach (var type in assembly.DefinedTypes)
            {
                foreach (var method in type.GetMethods(BindingFlags.Static))
                {
                    var attr = method.GetCustomAttributes().Select()
                }
            }
        }
    }
}
