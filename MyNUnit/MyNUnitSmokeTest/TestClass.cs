using System;
using System.Threading;
using MyNUnitLib;

namespace MyNUnitSmokeTest
{ 
    public class TestClass
    {
        [Test]
        public void TestMethod1()
        {
            throw new DivideByZeroException();
        }

        [Test]
        public void TestMethod2()
        {
            Thread.Sleep(100);
        }
    }
}
