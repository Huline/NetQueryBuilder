namespace NetQueryBuilder.Wpf.Tests.Mocks;

public static class TestData
{
    public static IQueryable<Person> GetPeople()
    {
        return new List<Person>
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", Email = "john@example.com", Age = 30, BirthDate = new DateTime(1993, 5, 15), IsActive = true, City = "New York", Country = "USA" },
            new() { Id = 2, FirstName = "Jane", LastName = "Smith", Email = "jane@example.com", Age = 25, BirthDate = new DateTime(1998, 8, 22), IsActive = true, City = "London", Country = "UK" },
            new() { Id = 3, FirstName = "Bob", LastName = "Johnson", Email = "bob@example.com", Age = 35, BirthDate = new DateTime(1988, 3, 10), IsActive = false, City = "Paris", Country = "France" }
        }.AsQueryable();
    }
}
