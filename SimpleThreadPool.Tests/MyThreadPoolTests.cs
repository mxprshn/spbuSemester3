using System;
using System.Threading;
using System.Collections.Concurrent;
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
    }
}
