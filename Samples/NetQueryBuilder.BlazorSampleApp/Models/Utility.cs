using System.ComponentModel.DataAnnotations.Schema;

namespace NetQueryBuilder.BlazorSampleApp.Models;

public class Utility
{
    public int UtilityId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int AddressId { get; set; }

    [ForeignKey(nameof(AddressId))] public virtual Address? Address { get; set; }
}