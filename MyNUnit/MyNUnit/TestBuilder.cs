using MyNUnitLib;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Class constructing tests for a special class.
    /// </summary>
    public class TestBuilder
    {
        private MethodInfo beforeMethod;
        private MethodInfo afterMethod;
        private ConstructorInfo constructor;
        private TypeInfo typeInfo;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="typeInfo">Type to build tests for.</param>
        /// <param name="beforeMethod">Method executed before each built test.</param>
        /// <param name="afterMethod">Method executed after each built test.</param>
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

        /// <summary>
        /// Constructs a test object for class with the given test method.
        /// </summary>
        /// <param name="testMethod">Method to invoke when test is run.</param>
        /// <returns>Constructed test object.</returns>
        public ITest BuildTest(MethodInfo testMethod) => new Test(testMethod, this);

        /// <summary>
        /// Class of the tests built by TestBuilder.
        /// </summary>
        private class Test : ITest
        {
            private TestBuilder builder;
            private MethodInfo testMethod;
            private Type exceptionType;

            /// <summary>
            /// Name of the class where the test has been declared.
            /// </summary>
            public string ClassName { get; private set; }

            /// <summary>
            /// Name of the test.
            /// </summary>
            public string Name { get; private set; }

            /// <summary>
            /// Result of the test or null if it was not run.
            /// </summary>
            public bool? IsPassed { get; private set; } = null;

            /// <summary>
            /// Shows if the test was ignored and not run.
            /// </summary>
            public bool IsIgnored { get; private set; }

            /// <summary>
            /// If the test was ignored, reason for ignoring.
            /// </summary>
            public string IgnoreReason { get; private set; }

            /// <summary>
            /// Time elapsed for the test.
            /// </summary>
            public TimeSpan RunTime { get; private set; } = TimeSpan.Zero;

            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="testMethod">Method to invoke when the test is run.</param>
            /// <param name="builder">Builder constructing the test.</param>
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

            /// <summary>
            /// Runs test and saves its results.
            /// </summary>
            public void Run()
            {
                if (IsPassed != null)
                {
                    throw new InvalidOperationException("Test has already been run.");
                }

                if (IsIgnored)
                {                    
                    return;
                }

                IsPassed = true;

                var testObject = builder.constructor.Invoke(null);
                builder.beforeMethod?.Invoke(testObject, null);

                var stopwatch = Stopwatch.StartNew();

                try
                {
                    testMethod.Invoke(testObject, null);
                }
                catch (TargetInvocationException exception) when (exception.InnerException.GetType() == exceptionType)
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