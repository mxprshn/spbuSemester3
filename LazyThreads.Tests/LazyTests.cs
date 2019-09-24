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
            private ILazy<T> testLazy;
            private readonly T expectedValue;
            private readonly Func<T> supplier;

            public LazyTester(Func<T> supplier, T expectedValue)
            {
                this.supplier = supplier;
                this.expectedValue = expectedValue;
            }

            public void SingleGetTest()
            {
                testLazy = LazyFactory.CreateLazy<T>(supplier);
                Assert.AreEqual(expectedValue, testLazy.Get());
            }

            public void MultipleGetTest()
            {
                const int GetAmount = 20;
                var count = 0;
                testLazy = LazyFactory.CreateLazy<T>(() => {
                    ++count;
                    return supplier();
                });

                for (var i = 0; i < GetAmount; ++i)
                {
                    Assert.AreEqual(expectedValue, testLazy.Get());
                }

                Assert.AreEqual(1, count);
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
            const int NewValue = 100;
            var testLazy = LazyFactory.CreateLazy<TestObject>(() => { return new TestObject(0); });
            var result = testLazy.Get();
            result.Value = NewValue;
            Assert.AreEqual(NewValue, testLazy.Get().Value);
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
