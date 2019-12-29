using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MyNUnitLib;

namespace MyNUnit
{
    /// <summary>
    /// Class running test methods in assemblies. 
    /// </summary>
    public static class TestRunner
    {
        /// <summary>
        /// Runs all tests in assembly.
        /// </summary>
        /// <param name="path">Path to assembly.</param>
        /// <returns>List of executed tests.</returns>
        public static IList<ITest> Test(string path)
        {
            var assembly = Assembly.Load(File.ReadAllBytes(path));
            return TestAssembly(assembly).ToList();
        }

        private static IList<ITest> TestAssembly(Assembly assembly) =>
                assembly.DefinedTypes.AsParallel().AsOrdered().SelectMany(t => TestType(t)).ToList();

        private static IList<ITest> TestType(TypeInfo typeInfo)
        {
            var myNUnitMethods = typeInfo.GetMethods().Where(MyNUnitMethodSelector<MyNUnitAttribute>);
            var testMethods = new List<ITest>();

            if (myNUnitMethods.Count() == 0)
            {
                return testMethods;
            }

            if (typeInfo.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new TestRunnerException("Type must have a constructor without parameters.");
            }

            var testBuilder = new TestBuilder(typeInfo, myNUnitMethods.Where(MyNUnitMethodSelector<BeforeAttribute>).FirstOrDefault(),
                        myNUnitMethods.Where(MyNUnitMethodSelector<AfterAttribute>).FirstOrDefault());

            foreach (var testMethod in myNUnitMethods.Where(MyNUnitMethodSelector<TestAttribute>))
            {
                testMethods.Add(testBuilder.BuildTest(testMethod));
            }

            myNUnitMethods.Where(MyNUnitMethodSelector<BeforeClassAttribute>).FirstOrDefault()?.Invoke(null, null);

            Parallel.ForEach(testMethods, m => m.Run());

            myNUnitMethods.Where(MyNUnitMethodSelector<AfterClassAttribute>).FirstOrDefault()?.Invoke(null, null);

            return testMethods;
        }

        private static bool MyNUnitMethodSelector<T>(MethodInfo methodInfo) where T : MyNUnitAttribute
        {
            var attributes = methodInfo.GetCustomAttributes<T>();

            if (attributes.Count() > 1)
            {
                throw new TestRunnerException("Method must have only one MyNUnitAttribute.");
            }
            else if (attributes.Count() == 0)
            {
                return false;
            }

            var attributeType = attributes.First().GetType();

            if (methodInfo.ReturnType != typeof(void))
            {
                throw new TestRunnerException($"Method {methodInfo.Name} with attribute {attributeType.Name} must return void.");
            }

            if (methodInfo.GetParameters().Count() != 0)
            {
                throw new TestRunnerException($"Method {methodInfo.Name} with attribute {attributeType.Name} must have no parameters.");
            }

            if (typeof(StaticMyNUnitAttribute).IsAssignableFrom(attributeType) && !methodInfo.IsStatic)
            {
                throw new TestRunnerException($"Method {methodInfo.Name} with attribute {attributeType.Name} must be static.");
            }

            return true;
        }
    }
}