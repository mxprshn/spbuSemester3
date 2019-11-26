using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnitLib
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    class BeforeClassAttribute : Attribute
    {
    }
}
