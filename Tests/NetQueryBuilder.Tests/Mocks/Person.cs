namespace NetQueryBuilder.Tests.Mocks;

public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public Address? Address { get; set; }
    public List<Contact> Contacts { get; set; } = [];
    public bool IsActive { get; set; }
}