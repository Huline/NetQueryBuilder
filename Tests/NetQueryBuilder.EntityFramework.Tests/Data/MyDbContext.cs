using Microsoft.EntityFrameworkCore;
using NetQueryBuilder.EntityFramework.Tests.Data.Models;

namespace NetQueryBuilder.EntityFramework.Tests.Data;

public class MyDbContext : DbContext
{
    public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<Utility> Utility { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Person>()
            .ToTable(nameof(Person))
            .HasMany(person => person.Addresses)
            .WithOne(address => address.Person)
            .HasForeignKey(address => address.PersonId);
    }
}