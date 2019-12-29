using Microsoft.EntityFrameworkCore;

namespace MyNUnitWeb.Models
{
    /// <summary>
    /// Test run history repository.
    /// </summary>
    public class TestArchive : DbContext
    {
        /// <summary>
        /// Tested assemblies.
        /// </summary>
        public DbSet<AssemblyModel> AssemblyModels { get; set; }

        /// <summary>
        /// Run tests.
        /// </summary>
        public DbSet<TestModel> TestModels { get; set; }

        public TestArchive(DbContextOptions<TestArchive> options)
                : base(options)
        {
        }
    }
}