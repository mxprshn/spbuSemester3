using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
