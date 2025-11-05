using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.AspNetCoreSampleApp.Models;

namespace NetQueryBuilder.AspNetCoreSampleApp.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Utility> Utilities { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Person>()
            .ToTable(nameof(Person))
            .HasMany(person => person.Addresses)
            .WithOne(address => address.Person)
            .HasForeignKey(address => address.PersonId);

        modelBuilder
            .Entity<Address>()
            .HasMany(address => address.Utilities)
            .WithOne(utility => utility.Address)
            .HasForeignKey(utility => utility.AddressId);
    }
}
