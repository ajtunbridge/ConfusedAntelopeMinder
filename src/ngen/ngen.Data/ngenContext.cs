#region Using directives

using System.Data.Entity;
using ngen.Data.Model;

#endregion

namespace ngen.Data
{
    public class ngenDbContext : DbContext
    {
        public ngenDbContext() : base("name=ngenConnectionString")
        {
        }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<DocumentVersion> DocumentVersions { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Part> Parts { get; set; }

        public DbSet<PartVersion> PartVersions { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<SystemRole> SystemRoles { get; set; }
    }
}