using MyNUnitLib;

namespace MyNUnitSmokeTest
{
    public class SmokeTestClass
    {
        [Test]
        public void Test1() { }

        [Test]
        public void Test2() { }

        [Test]
        public void Test3() { }

        [Test]
        public void Test4() { }

        public void NotATest() { }

        public int AlsoNotATest(int ololo) => 244;
    }
}