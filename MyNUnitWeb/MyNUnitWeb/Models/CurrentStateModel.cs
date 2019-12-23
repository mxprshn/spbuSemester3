using Microsoft.AspNetCore.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Model of current test runner state. 
    /// </summary>
    public class CurrentStateModel
    {
        private readonly IWebHostEnvironment environment;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="environment">Current environment.</param>
        public CurrentStateModel(IWebHostEnvironment environment) => this.environment = environment;

        /// <summary>
        /// Loaded assemblies.
        /// </summary>
        public IEnumerable<string> Assemblies => Directory.EnumerateFiles($"{environment.WebRootPath}/Temp").
                Select(f => Path.GetFileName(f));

        /// <summary>
        /// Recently run tests results.
        /// </summary>
        public List<TestModel> Tests = new List<TestModel>();
    }
}
