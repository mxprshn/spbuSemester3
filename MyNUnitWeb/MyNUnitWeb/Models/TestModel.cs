using System;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Model reprsenting a run test.
    /// </summary>
    public class TestModel
    {
        /// <summary>
        /// Test id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Test name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of the class containing test.
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Result of the test or null if it was not run.
        /// </summary>
        public bool? IsPassed { get; set; }

        /// <summary>
        /// Shows if the test was ignored and not run.
        /// </summary>
        public bool IsIgnored { get; set; }

        /// <summary>
        /// If the test was ignored, reason for ignoring.
        /// </summary>
        public string IgnoreReason { get; set; }

        /// <summary>
        /// Time elapsed for the test.
        /// </summary>
        public TimeSpan RunTime { get; set; }

        /// <summary>
        /// Assembly containing the test.
        /// </summary>
        public AssemblyModel AssemblyModel { get; set; }
    }
}