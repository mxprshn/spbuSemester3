using MyNUnitLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AnotherMyNUnitSmokeTest
{
    public class TestClass2
    {
        [Test]
        public void TestMethod3()
        {
            Thread.Sleep(100);
        }
    }
}
