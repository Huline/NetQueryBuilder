using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetQueryBuilder.EntityFramework.Tests.Data.Models;

public class Address
{
    [Key] public int AddressId { get; set; }

    public string PersonId { get; set; }
    public string City { get; set; }

    public bool IsPrimary { get; set; }

    [ForeignKey(nameof(PersonId))] public virtual Person Person { get; set; }

    public virtual List<Utility> Utilities { get; set; }
}