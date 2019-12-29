using System;
using System.Reflection;

namespace MyNUnitLib
{
    /// <summary>
    /// Attribute used for marking methods as MyNUnit tests.
    /// </summary>
    public sealed class TestAttribute : MyNUnitAttribute
    {
        private Type exceptionType = null;

        /// <summary>
        /// Type of expected exception.
        /// </summary>
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

        /// <summary>
        /// If set, test is ignored with the message.
        /// </summary>
        public string Ignore { get; set; }
    }
}