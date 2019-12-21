using System;

namespace MyNUnitLib
{
    /// <summary>
    /// Base class for MyNUnit attributes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public abstract class MyNUnitAttribute : Attribute
    {
    }
}