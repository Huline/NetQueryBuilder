using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.AspNetCore.Models;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Configurations;

namespace NetQueryBuilder.AspNetCore.Extensions;

/// <summary>
/// Extension methods for configuring the NetQueryBuilder middleware pipeline
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds NetQueryBuilder middleware to the application pipeline.
    /// This configures session state and static files required by NetQueryBuilder.
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    /// <remarks>
    /// This method should be called after UseRouting() and before MapRazorPages().
    /// It automatically configures:
    /// - Static files (for CSS and JavaScript assets)
    /// - Session state (for query state persistence)
    ///
    /// Example usage:
    /// <code>
    /// app.UseRouting();
    /// app.UseNetQueryBuilder();
    /// app.MapRazorPages();
    /// </code>
    /// </remarks>
    /// <exception cref="InvalidOperationException">
    /// Thrown when required services are not registered.
    /// Call AddNetQueryBuilder() or AddNetQueryBuilder&lt;TContext&gt;() in ConfigureServices first.
    /// </exception>
    public static IApplicationBuilder UseNetQueryBuilder(this IApplicationBuilder app)
    {
        if (app == null)
            throw new ArgumentNullException(nameof(app));

        // Validate required services are registered
        ValidateRequiredServices(app);

        // Enable static files (required for serving CSS/JS from the library)
        app.UseStaticFiles();

        // Enable session (required for query state persistence)
        app.UseSession();

        return app;
    }

    private static void ValidateRequiredServices(IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;

        // Check for NetQueryBuilderOptions (indicates AddNetQueryBuilder was called)
        var options = services.GetService<NetQueryBuilderOptions>();
        if (options == null)
        {
            throw new InvalidOperationException(
                "NetQueryBuilder services are not registered. " +
                "Call 'services.AddNetQueryBuilder()' or 'services.AddNetQueryBuilder<TContext>()' " +
                "in your ConfigureServices method before calling 'app.UseNetQueryBuilder()'.");
        }

        // Check for IQuerySessionService
        var sessionService = services.GetService<IQuerySessionService>();
        if (sessionService == null)
        {
            throw new InvalidOperationException(
                "IQuerySessionService is not registered. " +
                "This usually indicates an incomplete NetQueryBuilder setup. " +
                "Ensure 'services.AddNetQueryBuilder()' was called correctly.");
        }

        // Check for IQueryConfigurator
        var configurator = services.GetService<IQueryConfigurator>();
        if (configurator == null)
        {
            throw new InvalidOperationException(
                "IQueryConfigurator is not registered. " +
                "Either use 'services.AddNetQueryBuilder<TContext>()' which auto-registers the configurator, " +
                "or manually register an IQueryConfigurator implementation: " +
                "'services.AddScoped<IQueryConfigurator, EfQueryConfigurator<YourDbContext>>()'.");
        }
    }
}
