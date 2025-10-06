using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.WpfSample.Models;

namespace NetQueryBuilder.WpfSample.Data;

public class SampleDbContext : DbContext
{
    public SampleDbContext(DbContextOptions<SampleDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Utility> Utilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasMany(person => person.Addresses)
            .WithOne(address => address.Person)
            .HasForeignKey(address => address.PersonId);

        modelBuilder.Entity<Address>()
            .HasMany(address => address.Utilities)
            .WithOne(utility => utility.Address)
            .HasForeignKey(utility => utility.AddressId);
    }
}
