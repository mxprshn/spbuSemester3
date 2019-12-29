using System.Collections.Generic;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Model representing a tested assembly.
    /// </summary>
    public class AssemblyModel
    {
        /// <summary>
        /// Assembly id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Assembly name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Models of tests contained in the assembly.
        /// </summary>
        public virtual ICollection<TestModel> TestModels { get; set; } = new List<TestModel>();
    }
}