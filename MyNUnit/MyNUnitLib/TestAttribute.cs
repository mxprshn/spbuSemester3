using System;
using System.Reflection;

namespace MyNUnitLib
{
    public sealed class TestAttribute : MyNUnitAttribute
    {
        private Type exceptionType = null;

        public Type Expected
        {
            get => exceptionType;

            set
            {
                if (!typeof(Exception).GetTypeInfo().IsAssignableFrom(value.GetTypeInfo()))
                {
                    throw new ArgumentException("Expected exception must be derived from Exception.");
                }

                exceptionType = value;
            }
        }

        public string Ignore { get; set; }
    }
}
