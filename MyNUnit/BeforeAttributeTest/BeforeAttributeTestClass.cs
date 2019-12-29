using MyNUnitLib;
using System;

namespace BeforeAttributeTest
{
    public class BeforeAttributeTestClass
    {
        private volatile bool isBeforeLaunched = false;

        [Before]
        public void BeforeTest() => isBeforeLaunched = true;

        [Test]
        public void Test()
        {
            if (!isBeforeLaunched)
            {
                throw new Exception("ololo");
            }
        }
    }
}
