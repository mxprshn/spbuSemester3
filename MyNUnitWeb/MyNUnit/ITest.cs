using System;

namespace MyNUnit
{
    /// <summary>
    /// Interface representing a test with information about it.
    /// </summary>
    public interface ITest
    {
        /// <summary>
        /// Runs test and saves its results to properties.
        /// </summary>
        void Run();

        /// <summary>
        /// Name of the class where the test has been declared.
        /// </summary>
        string ClassName { get; }

        /// <summary>
        /// Name of the test.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Result of the test or null if it was not run.
        /// </summary>
        bool? IsPassed { get; }

        /// <summary>
        /// Shows if the test was ignored and not run.
        /// </summary>
        bool IsIgnored { get; }

        /// <summary>
        /// If the test was ignored, reason for ignoring.
        /// </summary>
        string IgnoreReason { get; }

        /// <summary>
        /// Time elapsed for the test.
        /// </summary>
        TimeSpan RunTime { get; }
    }
}