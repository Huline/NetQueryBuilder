using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NetQueryBuilder.AspNetCore.Models;

namespace NetQueryBuilder.AspNetCore.Services;

/// <summary>
/// Background service that periodically cleans up expired query sessions.
/// This prevents memory leaks from abandoned sessions.
/// </summary>
public class SessionCleanupService : BackgroundService
{
    private readonly IQuerySessionService _sessionService;
    private readonly NetQueryBuilderOptions _options;
    private readonly ILogger<SessionCleanupService> _logger;

    public SessionCleanupService(
        IQuerySessionService sessionService,
        NetQueryBuilderOptions options,
        ILogger<SessionCleanupService> logger)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("SessionCleanupService starting. Cleanup interval: {Interval}",
            _options.SessionCleanupInterval);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_options.SessionCleanupInterval, stoppingToken);

                if (_sessionService is ISessionCleanupSupport cleanupSupport)
                {
                    var removedCount = cleanupSupport.CleanupExpiredSessions(_options.SessionTimeout);
                    if (removedCount > 0)
                    {
                        _logger.LogInformation("SessionCleanupService: Removed {Count} expired sessions", removedCount);
                    }
                    else
                    {
                        _logger.LogDebug("SessionCleanupService: No expired sessions to remove");
                    }
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Graceful shutdown
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during session cleanup");
            }
        }

        _logger.LogInformation("SessionCleanupService stopped");
    }
}

/// <summary>
/// Interface for session services that support cleanup operations.
/// </summary>
public interface ISessionCleanupSupport
{
    /// <summary>
    /// Removes sessions that have been inactive longer than the specified timeout.
    /// </summary>
    /// <param name="timeout">The timeout after which sessions are considered expired</param>
    /// <returns>The number of sessions removed</returns>
    int CleanupExpiredSessions(TimeSpan timeout);
}
