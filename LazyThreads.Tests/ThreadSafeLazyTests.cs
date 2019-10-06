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
            void ConcurrentGetTest();
        }

        private class ThreadSafeLazyTester<T> : IThreadSafeLazyTester
        {
            private ILazy<T> testLazy;
            private readonly T expectedValue;
            private readonly Func<T> supplier;
            private readonly int threadAmount = 1;
            private readonly int getAmount = 1;

            public ThreadSafeLazyTester(Func<T> supplier, T expectedValue)
            {
                this.supplier = supplier;
                this.expectedValue = expectedValue;
            }

            public ThreadSafeLazyTester(Func<T> supplier, T expectedValue, int threadAmount = 1, int getAmount = 1)
                : this(supplier, expectedValue)
            {
                this.threadAmount = threadAmount;
                this.getAmount = getAmount;
            }

            public void SingleGetTest()
            {
                testLazy = LazyFactory.CreateThreadSafeLazy<T>(supplier);
                Assert.AreEqual(expectedValue, testLazy.Get());
            }

            public void MultipleGetTest()
            {
                const int GetAmount = 20;
                var count = 0;
                testLazy = LazyFactory.CreateThreadSafeLazy<T>(() => {
                    ++count;
                    return supplier();
                });

                for (var i = 0; i < GetAmount; ++i)
                {
                    Assert.AreEqual(expectedValue, testLazy.Get());
                }

                Assert.AreEqual(1, count);
            }

            public void ConcurrentGetTest()
            {
                var count = 0;
                var resetEvent = new ManualResetEvent(false);

                var testLazy = LazyFactory.CreateThreadSafeLazy<T>(() =>
                {
                    ++count;
                    return supplier();
                });

                var threads = new Thread[threadAmount];

                for (var i = 0; i < threads.Length; ++i)
                {
                    threads[i] = new Thread(() =>
                    {
                        resetEvent.WaitOne();
                        for (var j = 0; j < getAmount; ++j)
                        {
                            var result = testLazy.Get();
                            Assert.AreEqual(expectedValue, result);
                        }
                    });
                }

                for (var i = 0; i < threads.Length; ++i)
                {
                    threads[i].Start();
                }

                resetEvent.Set();

                for (var i = 0; i < threads.Length; ++i)
                {
                    threads[i].Join();
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
        public void SingleGetTest(IThreadSafeLazyTester tester) => tester.SingleGetTest();

        [TestCaseSource("GetTestCases")]
        public void MultipleGetTest(IThreadSafeLazyTester tester) => tester.MultipleGetTest();

        [TestCaseSource("ConcurrentGetTestCases"), Repeat(30)]
        public void ConcurrentGetTest(IThreadSafeLazyTester tester) => tester.ConcurrentGetTest();

        [Test]
        public void AreTheSameGetTest()
        {
            const int NewValue = 100;
            var testLazy = LazyFactory.CreateThreadSafeLazy<TestObject>(() => { return new TestObject(0); });
            var result = testLazy.Get();
            result.Value = NewValue;
            Assert.AreEqual(NewValue, testLazy.Get().Value);
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

        private static object[] ConcurrentGetTestCases =
        {
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 1, 1),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 2, 1),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 3, 1),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 4, 1),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 8, 1),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 2, 3),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 4, 3),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 8, 3),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 2, 10),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 4, 10),
            new ThreadSafeLazyTester<Int64>(() => { return 42; }, 42, 8, 10),

            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 1, 1),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 2, 1),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 3, 1),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 4, 1),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 8, 1),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 2, 3),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 4, 3),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 8, 3),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 2, 10),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 4, 10),
            new ThreadSafeLazyTester<string>(() => { return "ololo"; }, "ololo", 8, 10)
        };
    }
}
