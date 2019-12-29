using MyNUnitLib;
using System;
using System.Threading;

namespace BeforeClassAttributeTest
{
    public class BeforeClassAttributeClass
    {
        private static int beforeClassLaunchCount = 0;

        [BeforeClass]
        public static void BeforeClass() => Interlocked.Increment(ref beforeClassLaunchCount);
        
        [Test]
        public void Test1()
        {
            if (beforeClassLaunchCount != 1)
            {
                throw new Exception("ololo");
            }
        }

        [Test]
        public void Test2()
        {
            if (beforeClassLaunchCount != 1)
            {
                throw new Exception("ololo");
            }
        }

        [Test]
        public void Test3()
        {
            if (beforeClassLaunchCount != 1)
            {
                throw new Exception("ololo");
            }
        }
    }
}