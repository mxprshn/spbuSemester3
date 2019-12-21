using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNUnitWeb.Models
{
    public class AssemblyModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<TestModel> Tests { get; set; }
    }
}
