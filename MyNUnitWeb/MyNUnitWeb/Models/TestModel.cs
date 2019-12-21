using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNUnitWeb.Models
{
    public class TestModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ClassName { get; set; }
        public  bool? IsPassed { get; set; }
        public bool IsIgnored { get; set; }
        public string IgnoreReason { get; set; }
        public TimeSpan RunTime { get; set; }
        public AssemblyModel Assembly { get; set; }
    }
}