using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.AspNetCore.Models;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFramework;

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

        // Register session cleanup service if enabled
        if (options.EnableSessionCleanup && options.SessionCleanupInterval > TimeSpan.Zero)
        {
            services.AddHostedService<SessionCleanupService>();
        }

        return services;
    }

    /// <summary>
    /// Adds NetQueryBuilder services with Entity Framework Core integration to the specified IServiceCollection.
    /// This method automatically registers the EfQueryConfigurator for the specified DbContext type.
    /// </summary>
    /// <typeparam name="TContext">The DbContext type to use for query building</typeparam>
    /// <param name="services">The IServiceCollection to add services to</param>
    /// <param name="configure">Optional configuration action</param>
    /// <returns>The IServiceCollection so that additional calls can be chained</returns>
    /// <remarks>
    /// This is the recommended way to set up NetQueryBuilder with Entity Framework Core.
    /// It registers all required services including the IQueryConfigurator.
    ///
    /// Example usage:
    /// <code>
    /// builder.Services.AddDbContext&lt;AppDbContext&gt;(...);
    /// builder.Services.AddNetQueryBuilder&lt;AppDbContext&gt;();
    /// </code>
    /// </remarks>
    public static IServiceCollection AddNetQueryBuilder<TContext>(
        this IServiceCollection services,
        Action<NetQueryBuilderOptions>? configure = null)
        where TContext : DbContext
    {
        if (services == null)
            throw new ArgumentNullException(nameof(services));

        // Add base NetQueryBuilder services
        services.AddNetQueryBuilder(configure);

        // Register EfQueryConfigurator for the specified DbContext
        // Scoped lifetime ensures a new configurator is created for each request,
        // which is important because DbContext is also scoped
        services.AddScoped<IQueryConfigurator, EfQueryConfigurator<TContext>>();

        return services;
    }
}
