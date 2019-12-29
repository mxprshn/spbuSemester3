using MyNUnitLib;
using System;

namespace ExpectedAttributeTest
{
    public class ExpectedAttributeTestClass
    {
        [Test(Expected = typeof(DivideByZeroException))]
        public void Test() => throw new DivideByZeroException();
    }
}