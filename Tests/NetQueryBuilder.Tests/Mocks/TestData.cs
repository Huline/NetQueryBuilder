namespace NetQueryBuilder.Tests.Mocks;

public static class TestData
{
    public static IQueryable<Person> GetPeople()
    {
        return new List<Person>
        {
            new()
            {
                Id = 1,
                FirstName = "Jean",
                LastName = "Dupont",
                BirthDate = new DateTime(1980, 5, 15),
                IsActive = true,
                Address = new Address
                {
                    Street = "123 Rue Principale",
                    City = "Paris",
                    ZipCode = "75001",
                    Country = "France"
                },
                Contacts = new List<Contact>
                {
                    new() { Type = "Email", Value = "jean.dupont@example.com" },
                    new() { Type = "Phone", Value = "0123456789" }
                }
            },
            new()
            {
                Id = 2,
                FirstName = "Marie",
                LastName = "Martin",
                BirthDate = new DateTime(1992, 8, 22),
                IsActive = true,
                Address = new Address
                {
                    Street = "45 Avenue de la République",
                    City = "Lyon",
                    ZipCode = "69001",
                    Country = "France"
                }
            },
            new()
            {
                Id = 3,
                FirstName = "Pierre",
                LastName = "Durand",
                BirthDate = new DateTime(1975, 3, 10),
                IsActive = false,
                Address = new Address
                {
                    Street = "45 Avenue de la République",
                    City = "Lyon",
                    ZipCode = "69001",
                    Country = "France"
                }
            }
        }.AsQueryable();
    }
}