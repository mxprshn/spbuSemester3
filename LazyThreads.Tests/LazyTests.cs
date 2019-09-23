using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace LazyThreads.Tests
{
    [TestFixture]
    public class LazyTests
    {
        public interface ILazyTester
        {
            void SingleGetTest();
            void MultipleGetTest();
        }

        private class LazyTester<T> : ILazyTester
        {
            private Lazy<T> testLazy;
            private readonly T expectedValue;
            private readonly Func<T> supplier;

            public LazyTester(Func<T> supplier, T expectedValue)
            {
                this.supplier = supplier;
                this.expectedValue = expectedValue;
            }

            public void SingleGetTest()
            {
                testLazy = new Lazy<T>(supplier);
                var result = testLazy.Get();
                Assert.AreEqual(expectedValue, result);
            }

            public void MultipleGetTest()
            {
                var count = 0;
                testLazy = new Lazy<T>(() => {
                    Assert.IsTrue(count == 0);
                    ++count;
                    return supplier();
                });

                var result1 = testLazy.Get();
                var result2 = testLazy.Get();

                for (var i = 0; i < 20; ++i)
                {
                    _ = testLazy.Get();
                }

                Assert.AreEqual(expectedValue, result1);
                Assert.AreEqual(result1, result2);
            }
        }

        private class TestObject
        {
            public int Value { get; set; }

            public TestObject(int value)
            {
                Value = value;
            }

            public override bool Equals(object obj)
            {
                if ((obj == null) || !this.GetType().Equals(obj.GetType()))
                {
                    return false;
                }

                var baseTestObject = (TestObject)obj;
                return baseTestObject.Value == Value;
            }
        }

        [TestCaseSource("GetTestCases")]
        public void SingleGetTest(ILazyTester tester) => tester.SingleGetTest();

        [TestCaseSource("GetTestCases")]
        public void MultipleGetTest(ILazyTester tester) => tester.MultipleGetTest();

        [Test]
        public void AreTheSameGetTest()
        {
            var testLazy = new Lazy<TestObject>(() => { return new TestObject(0); });
            var result1 = testLazy.Get();
            result1.Value = 100;
            var result2 = testLazy.Get();
            Assert.AreEqual(100, result2.Value);
        }

        private static object[] GetTestCases =
        {
            new LazyTester<int>(() => { return 0; }, 0),
            new LazyTester<int>(() => { return 25 % 10; }, 5),
            new LazyTester<bool>(() => { return false; }, false),
            new LazyTester<string>(() => { return "ololo"; }, "ololo"),
            new LazyTester<string>(() => { return null; }, null),
            new LazyTester<List<int>>(() => { return new List<int>{ 1, 2, 3, 4, 5 }; }, new List<int>{ 1, 2, 3, 4, 5 }),
            new LazyTester<TestObject>(() => { return new TestObject(10); }, new TestObject(10))
        };
    }
}
