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
                    _ = AssemblyName.GetAssemblyName(name);                    
                }
                catch (Exception e)
                {
                    logger.Error(e, $"Invalid executable file: {name}");
                    continue;
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

            var testMethods = new Queue<(MethodInfo method, Type exception)>();

            var methods = new Dictionary<Type, MethodInfo>();
            var staticMethodAttr = new HashSet<Type> { typeof(BeforeClassAttribute), typeof(AfterClassAttribute) };
            var methodAttr = new HashSet<Type> { typeof(BeforeAttribute), typeof(AfterAttribute),
                    typeof(BeforeClassAttribute), typeof(AfterClassAttribute), typeof(TestAttribute) };

            foreach (var method in typeInfo.GetMethods())
            {
                var goodAttributes = methodAttr.Intersect(method.GetCustomAttributes().Select(a => a.GetType()));

                if (goodAttributes.Count() != 1)
                {
                    if (goodAttributes.Count() > 1)
                    {
                        logger.Error("Method must have only one test attribute.");
                    }
                    break;
                }

                var attribute = goodAttributes.First();

                if (attribute == typeof(TestAttribute))
                {
                    if (()attribute)
                }
                else if (staticMethodAttr.Contains(attribute) && !method.IsStatic)
                {
                    logger.Error("Method must be static.");
                }
                else if (!methods.ContainsKey(attribute))
                {
                    methods.Add(attribute, method);
                }
                else
                {
                    logger.Error($"Class must have only one method with {attribute.Name} attribute.");
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
