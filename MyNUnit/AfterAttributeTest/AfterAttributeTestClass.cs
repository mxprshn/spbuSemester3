using MyNUnitLib;
using System;
using System.Threading;

namespace AfterAttributeTest
{
    public class AfterAttributeTestClass
    {
        private static volatile bool isAfterLaunched = false;

        [Test]
        public void Test1()
        {
            if (!isAfterLaunched)
            {
                throw new Exception("ololo");
            }
        }

        [Test]
        public void Test2()
        {
            Thread.Sleep(500);

            if (!isAfterLaunched)
            {
                throw new Exception("ololo");
            }
        }

        [After]
        public void After() => isAfterLaunched = true;
    }
}