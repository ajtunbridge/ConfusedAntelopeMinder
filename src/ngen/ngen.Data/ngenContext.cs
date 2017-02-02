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

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ClientSetting>().MapToStoredProcedures();

            modelBuilder.Entity<Customer>().MapToStoredProcedures();

            modelBuilder.Entity<Document>().MapToStoredProcedures();

            modelBuilder.Entity<DocumentFolder>().MapToStoredProcedures();

            modelBuilder.Entity<DocumentVersion>().MapToStoredProcedures();

            modelBuilder.Entity<Employee>().MapToStoredProcedures();

            modelBuilder.Entity<Fixture>().MapToStoredProcedures();

            modelBuilder.Entity<Operation>().MapToStoredProcedures();

            modelBuilder.Entity<Part>().MapToStoredProcedures();

            modelBuilder.Entity<PartVersion>().MapToStoredProcedures();

            modelBuilder.Entity<Person>().MapToStoredProcedures();

            modelBuilder.Entity<Photo>().MapToStoredProcedures();

            modelBuilder.Entity<ProductionMethod>().MapToStoredProcedures();

            modelBuilder.Entity<Supplier>().MapToStoredProcedures();

            modelBuilder.Entity<SystemRole>().MapToStoredProcedures();

            modelBuilder.Entity<WorkCentre>().MapToStoredProcedures();

            modelBuilder.Entity<WorkCentreGroup>().MapToStoredProcedures();

            modelBuilder.Entity<WorkCentreGroup>()
                .HasOptional(e => e.Parent)
                .WithMany()
                .HasForeignKey(m => m.ParentGroupId);
        }

        public DbSet<ClientSetting> ClientSettings { get; set; }

        public DbSet<Customer> Customers { get; set; }

        public DbSet<Document> Documents { get; set; }

        public DbSet<DocumentFolder> DocumentFolders { get; set; }

        public DbSet<DocumentVersion> DocumentVersions { get; set; }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<Fixture> Fixtures { get; set; }

        public DbSet<Part> Parts { get; set; }

        public DbSet<PartVersion> PartVersions { get; set; }

        public DbSet<Person> People { get; set; }

        public DbSet<Photo> Photos { get; set; }

        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<SystemRole> SystemRoles { get; set; }

        public DbSet<WorkCentre> WorkCentres { get; set; }

        public DbSet<WorkCentreGroup> WorkCentreGroups { get; set; }
    }
}