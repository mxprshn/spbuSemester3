﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnitLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class TestAttribute : Attribute
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
