namespace NetQueryBuilder.AspNetCore.Models;

/// <summary>
/// View model for rendering and binding condition forms
/// </summary>
public class ConditionViewModel
{
    /// <summary>
    /// Unique identifier for the condition
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Property path being queried
    /// </summary>
    public string PropertyPath { get; set; } = string.Empty;

    /// <summary>
    /// Operator to apply
    /// </summary>
    public string Operator { get; set; } = string.Empty;

    /// <summary>
    /// Value to compare against
    /// </summary>
    public string? Value { get; set; }

    /// <summary>
    /// Logical operator to use before this condition (And/Or)
    /// </summary>
    public string LogicalOperator { get; set; } = "And";

    /// <summary>
    /// Whether this is a block condition (group)
    /// </summary>
    public bool IsBlock { get; set; }

    /// <summary>
    /// Nesting level (for indentation)
    /// </summary>
    public int Level { get; set; }

    /// <summary>
    /// Parent condition ID (for nested conditions)
    /// </summary>
    public string? ParentId { get; set; }

    /// <summary>
    /// Child conditions (for block conditions)
    /// </summary>
    public List<ConditionViewModel> Children { get; set; } = new();
}
