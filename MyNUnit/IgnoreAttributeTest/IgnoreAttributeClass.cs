using MyNUnitLib;
using System;

namespace IgnoreAttributeTest
{
    public class IgnoreAttributeClass
    {
        [Test(Ignore = "ololo")]
        public void IgnoredTest() => throw new DivideByZeroException();
    }
}
