using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNUnitWeb.Models
{
    public class TestRunnerRepository : DbContext
    {
        public DbSet<AssemblyModel> AssemblyFileModels { get; set; }

        public TestRunnerRepository(DbContextOptions<TestRunnerRepository> options)
                : base(options)
        {
        }
    }
}