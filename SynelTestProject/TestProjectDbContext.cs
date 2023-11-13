using Microsoft.EntityFrameworkCore;
using SynelTestProject.Models;

namespace SynelTestProject
{
    public class TestProjectDbContext:DbContext
    {
        public TestProjectDbContext(DbContextOptions<TestProjectDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }
    }
}
