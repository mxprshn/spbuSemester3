using MyNUnitLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MyNUnit
{
    public class TestBuilder
    {
        private MethodInfo beforeMethod;
        private MethodInfo afterMethod;
        private ConstructorInfo constructor;
        private TypeInfo typeInfo;

        public TestBuilder(TypeInfo typeInfo, MethodInfo beforeMethod = null, MethodInfo afterMethod = null)
        {
            constructor = typeInfo.GetConstructor(Type.EmptyTypes);

            if (constructor == null)
            {
                throw new ArgumentException("Type must have a constructor without parameters.");
            }

            if (beforeMethod != null)
            {
                if (beforeMethod.GetCustomAttributes().Where(a => a.GetType() == typeof(BeforeAttribute)).Count() == 0)
                {
                    throw new ArgumentException("BeforeMethod must have BeforeAttribute.");
                }

                if (beforeMethod.ReturnType != typeof(void))
                {
                    throw new ArgumentException("BeforeMethod must return void.");
                }

                if (beforeMethod.GetParameters().Count() != 0)
                {
                    throw new ArgumentException("BeforeMethod must have no parameters.");
                }
            }

            if (afterMethod != null)
            {
                if (afterMethod.GetCustomAttributes().Where(a => a.GetType() == typeof(AfterAttribute)).Count() == 0)
                {
                    throw new ArgumentException("AfterMethod must have AfterAttribute.");
                }

                if (afterMethod.ReturnType != typeof(void))
                {
                    throw new ArgumentException("AfterMethod must return void.");
                }

                if (afterMethod.GetParameters().Count() != 0)
                {
                    throw new ArgumentException("AfterMethod must have no parameters.");
                }
            }

            this.beforeMethod = beforeMethod;
            this.afterMethod = afterMethod;
            this.typeInfo = typeInfo;
        }

        public ITest BuildTest(MethodInfo testMethod) => new Test(testMethod, this);

        private class Test : ITest
        {
            private TestBuilder builder;
            private MethodInfo testMethod;
            private Type exceptionType;

            public string ClassName { get; private set; }
            public string Name { get; private set; }
            public bool? IsPassed { get; private set; } = null;
            public bool IsIgnored { get; private set; }
            public string IgnoreReason { get; private set; }
            public TimeSpan RunTime { get; private set; } = TimeSpan.Zero;

            public Test(MethodInfo testMethod, TestBuilder builder)
            {
                var testAttribute = testMethod.GetCustomAttributes().Where(a => a.GetType() == typeof(TestAttribute)).First() as TestAttribute;

                if (testAttribute == null)
                {
                    throw new ArgumentException("TestMethod argument must have TestAttribute.");
                }

                this.testMethod = testMethod;
                this.builder = builder;
                ClassName = builder.typeInfo.Name;
                Name = testMethod.Name;
                IsIgnored = testAttribute.Ignore != null;
                IgnoreReason = testAttribute.Ignore;
                exceptionType = testAttribute.Expected;
            }

            public void Run()
            {
                if (IsPassed != null)
                {
                    throw new InvalidOperationException("Test has already been run.");
                }

                IsPassed = true;

                if (IsIgnored)
                {                    
                    return;
                }

                var testObject = builder.constructor.Invoke(null);
                builder.beforeMethod?.Invoke(testObject, null);

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    testMethod.Invoke(testObject, null);
                }
                catch (Exception exception) when (exception.GetType() == exceptionType)
                {

                }
                catch
                {
                    IsPassed = false;
                }

                stopwatch.Stop();
                RunTime = stopwatch.Elapsed;

                builder.afterMethod?.Invoke(testObject, null);

                builder = null;
                testMethod = null;
            }
        }
    }
}