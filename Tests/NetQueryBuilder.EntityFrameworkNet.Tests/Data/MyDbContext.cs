using System.Data.Entity;
using NetQueryBuilder.EntityFrameworkNet.Tests.Data.Models;

namespace NetQueryBuilder.EntityFrameworkNet.Tests.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base("MyDbContext")
        {
        }

        public DbSet<Person> Persons { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Utility> Utility { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Table pour Person
            modelBuilder.Entity<Person>()
                .ToTable("Person") // Table nommée "Person"
                .HasKey(p => p.PersonId);

            // Table pour Address
            modelBuilder.Entity<Address>()
                .ToTable("Address") // Table nommée "Address"
                .HasKey(a => a.AddressId);

            // Relation Person → Addresses (1 à plusieurs)
            modelBuilder.Entity<Person>()
                .HasMany(p => p.Addresses)
                .WithRequired(a => a.Person)
                .HasForeignKey(a => a.PersonId);
        }
    }
}