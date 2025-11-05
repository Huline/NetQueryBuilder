using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.AspNetCore.Models;
using NetQueryBuilder.AspNetCore.Services;

namespace NetQueryBuilder.AspNetCore.Extensions;

/// <summary>
/// Extension methods for setting up NetQueryBuilder services in an IServiceCollection
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds NetQueryBuilder services to the specified IServiceCollection
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to</param>
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The IServiceCollection so that additional calls can be chained</returns>
    public static IServiceCollection AddNetQueryBuilder(
        this IServiceCollection services,
        Action<NetQueryBuilderOptions>? configure = null)
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // Configure options
        var options = new NetQueryBuilderOptions();
        configure?.Invoke(options);

        // Register options as singleton
        services.AddSingleton(options);

        // Register session service as singleton
        services.AddSingleton<IQuerySessionService, QuerySessionService>();

        // Add distributed memory cache for session support
        services.AddDistributedMemoryCache();

        // Configure session
        services.AddSession(sessionOptions =>
        {
            sessionOptions.IdleTimeout = options.SessionTimeout;
            sessionOptions.Cookie.HttpOnly = true;
            sessionOptions.Cookie.IsEssential = true;
            sessionOptions.Cookie.Name = ".NetQueryBuilder.Session";
        });

        return services;
    }
}
