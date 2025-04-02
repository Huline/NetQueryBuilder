using System.ComponentModel.DataAnnotations.Schema;

namespace NetQueryBuilder.EntityFramework.Tests.Data.Models;

public class Utility
{
    public int UtilityId { get; set; }
    public string Provider { get; set; }
    public string AccountNumber { get; set; }
    public string Type { get; set; }
    public int AddressId { get; set; }

    [ForeignKey(nameof(AddressId))] public virtual Address Address { get; set; }
}