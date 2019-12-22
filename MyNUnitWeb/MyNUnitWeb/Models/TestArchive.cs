using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyNUnitWeb.Models
{
    public class TestArchive : DbContext
    {
        public DbSet<AssemblyModel> AssemblyModels { get; set; }
        public DbSet<TestModel> TestModels { get; set; }

        public TestArchive(DbContextOptions<TestArchive> options)
                : base(options)
        {
        }
    }
}