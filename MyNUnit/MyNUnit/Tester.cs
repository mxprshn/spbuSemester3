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
            // Add also exe files
            var executableNames = Directory.EnumerateFiles(path, "*.dll", SearchOption.AllDirectories);            

            foreach (var name in executableNames)
            {
                // Remove catching everything
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
                var goodAttributes = method.GetCustomAttributes().Where(a => methodAttr.Contains(a.GetType()));

                if (goodAttributes.Count() != 1)
                {
                    if (goodAttributes.Count() > 1)
                    {
                        logger.Error("Method must have only one test attribute.");
                        return;
                    }

                    continue;
                }

                if (method.ReturnType != typeof(void))
                {
                    logger.Error("Method must return void.");
                    return;
                }

                if (method.GetParameters().Count() != 0)
                {
                    logger.Error("Method must have no parameters.");
                    return;
                }

                var attribute = goodAttributes.First();
                var attributeType = attribute.GetType();

                if (attribute is TestAttribute testAttribute)
                {
                    if (testAttribute.Ignore != null)
                    {
                        logger.Info($"Test method {method.Name} is ignored: {testAttribute.Ignore}");
                        continue;
                    }

                    testMethods.Enqueue((method, testAttribute.Expected));
                }
                else if (staticMethodAttr.Contains(attributeType) && !method.IsStatic)
                {
                    logger.Error("Method must be static.");
                    // Throw some exception
                }
                else if (!methods.ContainsKey(attributeType))
                {
                    methods.Add(attributeType, method);
                }
                else
                {
                    logger.Error($"Class must have only one method with {attributeType.Name} attribute.");
                    // Throw some exception
                }
            }
            
            if (testMethods.Count == 0)
            {
                logger.Info($"Tests not found in class {typeInfo.Name}");
                return;
            }

            if (methods.TryGetValue(typeof(BeforeClassAttribute), out var beforeClassMethod))
            {
                beforeClassMethod.Invoke(null, null);
            }

            methods.TryGetValue(typeof(BeforeAttribute), out var beforeMethod);
            methods.TryGetValue(typeof(AfterAttribute), out var afterMethod);

            Parallel.ForEach(testMethods, i =>
            {
                var testClassObject = constructor.Invoke(null);

                if (beforeMethod != null)
                {
                    beforeMethod.Invoke(testClassObject, null);
                }

                TestMethod(testClassObject, i.method, i.exception);

                if (afterMethod != null)
                {
                    afterMethod.Invoke(testClassObject, null);
                }
            });

            if (methods.TryGetValue(typeof(BeforeClassAttribute), out var afterClassMethod))
            {
                afterClassMethod.Invoke(null, null);
            }
        }

        private static void TestMethod(object testObject, MethodInfo methodInfo, Type exceptionType)
        {
            try
            {
                methodInfo.Invoke(testObject, null);
            }
            catch (Exception exception) when (exception.GetType() == exceptionType)
            {

            }
            catch
            {
                logger.Info($"Test {methodInfo.Name} failed.");
                return;
            }

            logger.Info($"Test {methodInfo.Name} passed.");
        }
    }
}
