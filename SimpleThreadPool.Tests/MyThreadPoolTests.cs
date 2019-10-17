using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace SimpleThreadPool.Tests
{
    [TestFixture]
    public class MyThreadPoolTests
    {
        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(20)]
        [Repeat(3)]
        public void ThreadCountTest(int threadCount)
        {
            var pool = new MyThreadPool(threadCount);
            var idBag = new ConcurrentBag<int>();
            var handles = new WaitHandle[threadCount];

            for (var i = 0; i < threadCount; ++i)
            {
                handles[i] = new AutoResetEvent(false);
                var localIndex = i;

                pool.QueueTask(() =>
                {
                    idBag.Add(Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);
                    var resetEvent = (AutoResetEvent)handles[localIndex];
                    resetEvent.Set();
                    return 0;
                });
            }

            WaitHandle.WaitAll(handles);
            Assert.IsTrue(idBag.Distinct().Count() == idBag.Count);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [Repeat(3)]
        public void OneThreadManyTasksTest(int taskCount)
        {
            var pool = new MyThreadPool(1);
            var tasks = new List<IMyTask<int>>();
            var idBag = new ConcurrentBag<int>();

            for (var i = 0; i < taskCount; ++i)
            {
                var localIndex = i;

                tasks.Add(pool.QueueTask(() =>
                {
                    idBag.Add(Thread.CurrentThread.ManagedThreadId);
                    Thread.Sleep(1000);
                    return localIndex;
                }));
            }

            for (var i = 0; i < taskCount; ++i)
            {
                Assert.AreEqual(i, tasks[i].Result);
            }

            Assert.IsTrue(idBag.Distinct().Count() == 1);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(20)]
        [Repeat(3)]
        public void SimpleConcurrentCalculationTest(int threadCount)
        {
            var pool = new MyThreadPool(threadCount);
            var tasks = new List<IMyTask<int>>();

            for (var i = 0; i < threadCount; ++i)
            {
                var localIndex = i;

                tasks.Add(pool.QueueTask(() =>
                {
                    Thread.Sleep(1000);
                    return localIndex;
                }));
            }

            for (var i = 0; i < threadCount; ++i)
            {
                Assert.AreEqual(i, tasks[i].Result);
            }            
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [Repeat(3)]
        public void IsCompletedTest(int threadCount)
        {
            var pool = new MyThreadPool(threadCount);
            var tasks = new List<IMyTask<int>>();
            var resetEvent = new ManualResetEvent(false);

            for (var i = 0; i < threadCount; ++i)
            {
                var localIndex = i;

                tasks.Add(pool.QueueTask(() =>
                {
                    resetEvent.WaitOne();
                    return 0;
                }));
            }

            foreach (var task in tasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }

            resetEvent.Set();

            foreach (var task in tasks)
            {
                Assert.AreEqual(0, task.Result);
                Assert.IsTrue(task.IsCompleted);
            }
        }

        [TestCase(1)]
        [TestCase(5)]
        [TestCase(10)]
        [Repeat(3)]
        public void ConcurrentResultTest(int threadCount)
        {
            var tasks = new Task<int>[threadCount];
            var pool = new MyThreadPool(1);
            var resetEvent = new ManualResetEvent(false);

            var myTask = pool.QueueTask(() =>
            {
                Thread.Sleep(1000);
                return 0;
            });

            for (var i = 0; i < tasks.Count(); ++i)
            {
                tasks[i] = new Task<int>(() => 
                {
                    resetEvent.WaitOne();
                    return myTask.Result;
                });

                tasks[i].Start();
            }

            resetEvent.Set();

            foreach (var current in tasks)
            {
                Assert.AreEqual(0, current.Result);
            }
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [Repeat(3)]
        public void ShutdownTest(int threadCount)
        {
            var pool = new MyThreadPool(threadCount);
            var tasks = new List<IMyTask<int>>();
            var resetEvent = new ManualResetEvent(false);
            var counter = 0;
            for (var i = 0; i < threadCount + 10; ++i)
            {
                tasks.Add(pool.QueueTask(() =>
                {
                    resetEvent.WaitOne();
                    Interlocked.Increment(ref counter);
                    return 0;
                }));
            }

            Thread.Sleep(1000);
            pool.Shutdown();
            resetEvent.Set();
            Thread.Sleep(1000);
            Assert.AreEqual(threadCount, counter);

            for (var i = 0; i < threadCount; ++i)
            {
                Assert.AreEqual(0, tasks[i].Result);
                Assert.IsTrue(tasks[i].IsCompleted);
            }

            for (var i = threadCount; i < threadCount + 10; ++i)
            {
                Assert.Throws<ThreadPoolShutdownException>(() => _ = tasks[i].Result);
                Assert.IsFalse(tasks[i].IsCompleted);
            }

            Assert.Throws<ThreadPoolShutdownException>(() => pool.QueueTask(() => 0));
            Assert.Throws<ThreadPoolShutdownException>(() => tasks[0].ContinueWith((i) => i * 2));

            while (pool.ActiveThreadCount != 0);
        }

        [TestCase(1)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [Repeat(3)]
        public void ContinueWithShutdownTest(int threadCount)
        {
            var pool = new MyThreadPool(threadCount);
            var tasks = new IMyTask<int>[threadCount + 10];
            var resetEvent = new ManualResetEvent(false);

            tasks[0] = pool.QueueTask(() =>
            {
                resetEvent.WaitOne();
                return 1;
            });

            for (var i = 1; i < tasks.Length; ++i)
            {
                tasks[i] = tasks[i - 1].ContinueWith((j) =>
                {
                    return ++j;
                });
            }

            pool.Shutdown();
            resetEvent.Set();

            for (var i = 1; i < tasks.Length; ++i)
            {
                Assert.Throws<ThreadPoolShutdownException>(() => _ = tasks[i].Result);
                Assert.IsFalse(tasks[i].IsCompleted);
            }

            while (pool.ActiveThreadCount != 0);
        }

        [Test, Combinatorial]
        public void ContinueWithTest([Values(1, 3, 5, 10)] int threadCount,
                [Values(5, 10, 20)] int taskCount)
        {
            var pool = new MyThreadPool(threadCount);
            var tasks = new IMyTask<int>[taskCount];
            var counters = new int[taskCount];
            var resetEvent = new ManualResetEvent(false);

            tasks[0] = pool.QueueTask(() => 
            {
                resetEvent.WaitOne();
                Interlocked.Increment(ref counters[0]);
                return 1;
            });

            for (var i = 1; i < taskCount; ++i)
            {
                var localIndex = i;

                tasks[i] = tasks[i - 1].ContinueWith((j) => 
                {
                    resetEvent.WaitOne();
                    Interlocked.Increment(ref counters[localIndex]);
                    return j * 2;
                });
            }

            resetEvent.Set();

            for (var i = 0; i < taskCount; ++i)
            {
                Assert.AreEqual(tasks[i].Result, Math.Pow(2, i));
                Assert.AreEqual(1, counters[i]);
            }
        }

        [Test]
        public void MultipleContinueWithTest()
        {
            var pool = new MyThreadPool(3);
            var resetEvent1 = new ManualResetEvent(false);
            var resetEvent2 = new ManualResetEvent(false);

            var firstTask = pool.QueueTask(() =>
            {
                resetEvent1.WaitOne();
                return 100;
            });

            var intTask1 = firstTask.ContinueWith((j) => 
            {
                resetEvent2.WaitOne();
                return j + 100;
            });

            var intTask2 = firstTask.ContinueWith((j) =>
            {
                resetEvent2.WaitOne();
                return j + 200;
            });

            var stringTask = firstTask.ContinueWith((j) =>
            {
                resetEvent2.WaitOne();
                return j.ToString();
            });

            var boolTask = firstTask.ContinueWith((j) =>
            {
                resetEvent2.WaitOne();
                return j > 0;
            });

            resetEvent1.Set();

            Assert.AreEqual(100, firstTask.Result);

            Assert.IsFalse(intTask1.IsCompleted);
            Assert.IsFalse(intTask2.IsCompleted);
            Assert.IsFalse(stringTask.IsCompleted);
            Assert.IsFalse(boolTask.IsCompleted);

            resetEvent2.Set();

            Assert.AreEqual(200, intTask1.Result);
            Assert.AreEqual(300, intTask2.Result);
            Assert.AreEqual("100", stringTask.Result);
            Assert.AreEqual(true, boolTask.Result);

            Assert.IsTrue(intTask1.IsCompleted);
            Assert.IsTrue(intTask2.IsCompleted);
            Assert.IsTrue(stringTask.IsCompleted);
            Assert.IsTrue(boolTask.IsCompleted);
        }
    }
}