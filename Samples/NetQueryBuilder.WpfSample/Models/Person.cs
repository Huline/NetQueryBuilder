namespace NetQueryBuilder.WpfSample.Models;

/// <summary>
/// Sample entity for demonstrating the Query Builder.
/// </summary>
public class Person
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int Age { get; set; }
    public DateTime BirthDate { get; set; }
    public bool IsActive { get; set; }
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
}
