using MyNUnitLib;

namespace InvalidTestMethodsTest
{
    public class InvalidTestMethodsTestClass
    {
        [Test]
        public int Test1() => 244;

        [Test]
        public void Test2(int ololo) { }
    }
}
