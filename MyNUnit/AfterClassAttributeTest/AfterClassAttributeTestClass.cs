using MyNUnitLib;

namespace AfterClassAttributeTest
{
    public class AfterClassAttributeTestClass
    {
        private static int counter = 0;

        [Test]
        public void Test1() => ++counter;

        [Test]
        public void Test2() => ++counter;

        [Test]
        public void Test3() => ++counter;

        [AfterClass]
        public static void AfterClass()
        {
            AfterClassLaunchChecker.Counter = counter;
            AfterClassLaunchChecker.ResetEvent.Set();
        }
    }
}
