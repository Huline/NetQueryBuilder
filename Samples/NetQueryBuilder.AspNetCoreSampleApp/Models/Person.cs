using System.ComponentModel.DataAnnotations;

namespace NetQueryBuilder.AspNetCoreSampleApp.Models;

public class Person
{
    [Key] public required string PersonId { get; set; }

    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public int NumberOfChildren { get; set; }
    public bool IsAlive { get; set; }
    public DateTime Created { get; set; }
    public virtual List<Address> Addresses { get; set; } = [];
}
