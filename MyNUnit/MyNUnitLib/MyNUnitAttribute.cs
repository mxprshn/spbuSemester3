using System;

namespace MyNUnitLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class MyNUnitAttribute : Attribute
    {
    }
}
