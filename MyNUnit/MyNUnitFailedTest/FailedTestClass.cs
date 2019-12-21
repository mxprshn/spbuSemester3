using MyNUnitLib;
using System;

namespace MyNUnitFailedTest
{
    public class FailedTestClass
    {
        [Test]
        public void Test1() => throw new DivideByZeroException();

        [Test]
        public void Test2() => throw new Exception("ololo");
    }
}