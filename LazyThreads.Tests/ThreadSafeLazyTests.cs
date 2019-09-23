using System;
using System.Threading;
using System.Collections.Generic;
using NUnit.Framework;

namespace LazyThreads.Tests
{
    [TestFixture]
    public class ThreadSafeLazyTests
    {
        public interface IThreadSafeLazyTester
        {
            void SingleGetTest();
            void MultipleGetTest();
        }

        private class ThreadSafeLazyTester<T> : IThreadSafeLazyTester
        {
            private ThreadSafeLazy<T> testLazy;
            private readonly T expectedValue;
            private readonly Func<T> supplier;

            public ThreadSafeLazyTester(Func<T> supplier, T expectedValue)
            {
                this.supplier = supplier;
                this.expectedValue = expectedValue;
            }

            public void SingleGetTest()
            {
                testLazy = new ThreadSafeLazy<T>(supplier);
                var result = testLazy.Get();
                Assert.AreEqual(expectedValue, result);
            }

            public void MultipleGetTest()
            {
                var count = 0;
                testLazy = new ThreadSafeLazy<T>(() => {
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
        public void SingleGetTest(IThreadSafeLazyTester tester) => tester.SingleGetTest();

        [TestCaseSource("GetTestCases")]
        public void MultipleGetTest(IThreadSafeLazyTester tester) => tester.MultipleGetTest();

        [Test]
        public void AreTheSameGetTest()
        {
            var testLazy = new Lazy<TestObject>(() => { return new TestObject(0); });
            var result1 = testLazy.Get();
            result1.Value = 100;
            var result2 = testLazy.Get();
            Assert.AreEqual(100, result2.Value);
        }

        [Test]
        public void ConcurrentGetTest()
        {
            var count = 0;
            var testLazy = new Lazy<int>(() => {
                //Assert.IsTrue(count == 0);
                Console.WriteLine("ololo");
                ++count;
                return 0;
            });

            var threads = new Thread[10];

            for (var i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(() => { testLazy.Get(); });
            }

            for (var i = 0; i < threads.Length; ++i)
            {
                threads[i].Start();
            }

            testLazy.Get();
        }

        private static object[] GetTestCases =
        {
            new ThreadSafeLazyTester<int>(() => { return 0; }, 0),
            new ThreadSafeLazyTester<int>(() => { return 25 % 10; }, 5),
            new ThreadSafeLazyTester<bool>(() => { return false; }, false),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo"),
            new ThreadSafeLazyTester<string>(() => { return null; }, null),
            new ThreadSafeLazyTester<List<int>>(() => { return new List<int>{ 1, 2, 3, 4, 5 }; }, new List<int>{ 1, 2, 3, 4, 5 }),
            new ThreadSafeLazyTester<TestObject>(() => { return new TestObject(10); }, new TestObject(10))
        };
    }
}
