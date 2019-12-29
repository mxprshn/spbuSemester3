using MyNUnitLib;
using System;
using System.Threading;

namespace AfterClassAttributeTest
{
    class AfterClassLaunchChecker
    {
        public static int Counter
        {
            get => counter;
            set => counter = value;
        }

        private static volatile int counter = 0;
        public static ManualResetEvent ResetEvent { get; } = new ManualResetEvent(false);

        [Test]
        public void Test()
        {
            ResetEvent.WaitOne();

            if (counter != 3)
            {
                throw new Exception("ololo");
            }
        }
    }
}