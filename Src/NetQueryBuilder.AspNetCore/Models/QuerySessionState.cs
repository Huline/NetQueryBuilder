using NetQueryBuilder.Queries;

namespace NetQueryBuilder.AspNetCore.Models;

/// <summary>
///     Represents the state of a query building session
/// </summary>
public class QuerySessionState
{
    /// <summary>
    ///     Unique session identifier
    /// </summary>
    public string SessionId { get; set; } = string.Empty;

    /// <summary>
    ///     Currently selected entity type for querying
    /// </summary>
    public Type? SelectedEntityType { get; set; }

    /// <summary>
    ///     The active query instance
    /// </summary>
    public IQuery? Query { get; set; }

    /// <summary>
    ///     List of selected property paths for display (SELECT clause)
    /// </summary>
    public List<string> SelectedPropertyPaths { get; set; } = new();

    /// <summary>
    ///     Current query expression string (for display)
    /// </summary>
    public string? CurrentExpression { get; set; }

    /// <summary>
    ///     Query execution results (stored as object to handle generic types)
    /// </summary>
    public object? Results { get; set; }

    /// <summary>
    ///     Current page number for pagination
    /// </summary>
    public int CurrentPage { get; set; } = 1;

    /// <summary>
    ///     Number of items per page
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    ///     Total number of pages (calculated from results)
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    ///     Total number of items (calculated from results)
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    ///     Timestamp of the last access to this session.
    ///     Used for session cleanup and expiration.
    /// </summary>
    public DateTime LastAccessTime { get; set; } = DateTime.UtcNow;
}