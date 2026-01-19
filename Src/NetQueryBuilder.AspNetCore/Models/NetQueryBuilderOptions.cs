namespace NetQueryBuilder.AspNetCore.Models;

/// <summary>
/// Configuration options for NetQueryBuilder ASP.NET Core integration
/// </summary>
public class NetQueryBuilderOptions
{
    /// <summary>
    /// Session timeout duration. Default is 30 minutes.
    /// </summary>
    public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Default page size for query results. Default is 10.
    /// </summary>
    public int DefaultPageSize { get; set; } = 10;

    /// <summary>
    /// Maximum page size allowed. Default is 100.
    /// </summary>
    public int MaxPageSize { get; set; } = 100;

    /// <summary>
    /// Whether to enable session-based state storage. Default is true.
    /// </summary>
    public bool UseSessionStorage { get; set; } = true;

    /// <summary>
    /// Interval between session cleanup runs. Default is 5 minutes.
    /// Set to TimeSpan.Zero to disable automatic cleanup.
    /// </summary>
    public TimeSpan SessionCleanupInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Whether to enable automatic session cleanup. Default is true.
    /// </summary>
    public bool EnableSessionCleanup { get; set; } = true;
}
