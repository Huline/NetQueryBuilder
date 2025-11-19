namespace NetQueryBuilder.WpfSample.Models;

/// <summary>
/// Another sample entity for demonstrating entity selection.
/// </summary>
public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedDate { get; set; }
}
