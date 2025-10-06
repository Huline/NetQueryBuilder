using NetQueryBuilder.WpfSample.Models;

namespace NetQueryBuilder.WpfSample.Data;

public static class SampleDbContextExtensions
{
    public static async Task SeedDatabase(this SampleDbContext context)
    {
        if (!context.Persons.Any())
        {
            var persons = new List<Person>
            {
                new()
                {
                    FirstName = "Alice",
                    LastName = "Jones",
                    PersonId = "1",
                    NumberOfChildren = 2,
                    IsAlive = true,
                    Created = DateTime.Now.AddYears(-5)
                },
                new()
                {
                    FirstName = "Bob",
                    LastName = "Smith",
                    PersonId = "2",
                    NumberOfChildren = 0,
                    IsAlive = true,
                    Created = DateTime.Parse("2021-01-01")
                },
                new()
                {
                    FirstName = "Charlie",
                    LastName = "Brown",
                    PersonId = "3",
                    NumberOfChildren = 3,
                    IsAlive = false,
                    Created = DateTime.Parse("2019-06-15")
                },
                new()
                {
                    FirstName = "Diana",
                    LastName = "Prince",
                    PersonId = "4",
                    NumberOfChildren = 1,
                    IsAlive = true,
                    Created = DateTime.Now.AddYears(-2)
                }
            };

            context.Persons.AddRange(persons);
            await context.SaveChangesAsync();

            var addresses = new List<Address>
            {
                new()
                {
                    AddressId = 1,
                    PersonId = "1",
                    IsPrimary = true,
                    City = "Paris",
                    Utilities = new List<Utility>
                    {
                        new()
                        {
                            UtilityId = 1,
                            AccountNumber = "123456",
                            AddressId = 1,
                            Provider = "ConEd",
                            Type = "Electricity"
                        },
                        new()
                        {
                            UtilityId = 2,
                            AccountNumber = "789012",
                            AddressId = 1,
                            Provider = "Veolia",
                            Type = "Water"
                        }
                    }
                },
                new()
                {
                    AddressId = 2,
                    PersonId = "1",
                    IsPrimary = false,
                    City = "London"
                },
                new()
                {
                    AddressId = 3,
                    PersonId = "2",
                    IsPrimary = true,
                    City = "New York",
                    Utilities = new List<Utility>
                    {
                        new()
                        {
                            UtilityId = 3,
                            AccountNumber = "654321",
                            AddressId = 3,
                            Provider = "ConEd",
                            Type = "Electricity"
                        }
                    }
                },
                new()
                {
                    AddressId = 4,
                    PersonId = "3",
                    IsPrimary = true,
                    City = "Berlin",
                    Utilities = new List<Utility>
                    {
                        new()
                        {
                            UtilityId = 4,
                            AccountNumber = "111222",
                            AddressId = 4,
                            Provider = "Vattenfall",
                            Type = "Gas"
                        }
                    }
                },
                new()
                {
                    AddressId = 5,
                    PersonId = "4",
                    IsPrimary = true,
                    City = "Tokyo"
                }
            };

            context.Addresses.AddRange(addresses);
            await context.SaveChangesAsync();
        }
    }
}
