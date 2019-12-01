using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MyNUnitLib;

namespace MyNUnit
{
    public static class Tester
    {
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public static void Test(string path)
        {
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Target directory {path} does not exist.");
            }

            var assemblies = LoadAssemblies(path);

            if (assemblies.Count() == 0)
            {
                logger.Info("Assemblies not found.");
                return;
            }

            Parallel.ForEach(assemblies, TestAssembly);
        }
        
        private static IEnumerable<Assembly> LoadAssemblies(string path)
        {
            var result = new List<Assembly>();
            var executableNames = Directory.EnumerateFiles(path, "*.exe; *.dll", SearchOption.AllDirectories);            

            foreach (var name in executableNames)
            {
                try
                {
                }
                catch (Exception e)
                {
                    logger.Error(e, "Invalid executable file.");
                }

                result.Add(Assembly.LoadFrom(name));
            }

            return result;
        }

        private static void TestAssembly(Assembly assembly)
        {
            Parallel.ForEach(assembly.DefinedTypes, TestType);
        }

        private static void TestType(TypeInfo typeInfo)
        {
            var constructor = typeInfo.GetConstructor(Type.EmptyTypes);
            var resetEvent = new AutoResetEvent(true);

            if (constructor == null)
            {
                logger.Error("Tested class must have constructor without parameters.");
                return;
            }

            var beforeClassMethods = new List<MethodInfo>();
            var afterClassMethods = new List<MethodInfo>();
            var beforeMethods = new List<MethodInfo>();
            var afterMethods = new List<MethodInfo>();
            var testMethods = new Queue<(MethodInfo method, Type exception)>();

            foreach (var method in typeInfo.GetMethods())
            {
                foreach (var attribute in method.GetCustomAttributes())
                {
                    if (attribute.GetType() == typeof(BeforeClassAttribute))
                    {
                        if (!method.IsStatic)
                        {
                            logger.Error("'BeforeClass' method must be static.");
                        }
                        else
                        {
                            beforeClassMethods.Add(method);
                        }
                    }

                    if (attribute.GetType() == typeof(AfterClassAttribute))
                    {
                        if (!method.IsStatic)
                        {
                            logger.Error("'AfterClass' method must be static.");
                        }
                        else
                        {
                            afterClassMethods.Add(method);
                        } 
                    }

                    if (attribute.GetType() == typeof(BeforeAttribute))
                    {
                        beforeMethods.Add(method);
                    }

                    if (attribute.GetType() == typeof(AfterAttribute))
                    {
                        afterMethods.Add(method);
                    }

                    if (attribute.GetType() == typeof(TestAttribute))
                    {
                        var testAttribute = (TestAttribute)attribute;

                        if (testAttribute.Ignore != null)
                        {
                            logger.Info($"Test {method.Name} ignored: {testAttribute.Ignore}");
                        }
                        else
                        {
                            testMethods.Enqueue((method, testAttribute.Expected));
                        }
                    }
                }
            }

            if (testMethods.Count == 0)
            {
                logger.Info($"Tests not found in class {typeInfo.Name}");
                return;
            }

            foreach (var method in beforeClassMethods)
            {
                method.Invoke(null, null);
            }

            Parallel.ForEach(testMethods, i =>
            {
                resetEvent.WaitOne();
                var testClassObject = constructor.Invoke(null);
                TestMethod(i.method, i.exception);
            });

            foreach (var method in afterClassMethods)
            {
                method.Invoke(null, null);
            }
        }

        private static void TestMethod(MethodInfo methodInfo, Type exceptionType)
        {

        }
    }
}
