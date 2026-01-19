using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetQueryBuilder.AspNetCore.Services;

namespace NetQueryBuilder.AspNetCore.ViewComponents;

/// <summary>
///     View component for displaying query results in a table
/// </summary>
public class QueryResultsViewComponent : ViewComponent
{
    private readonly IQuerySessionService _sessionService;
    private readonly ILogger<QueryResultsViewComponent> _logger;

    public QueryResultsViewComponent(
        IQuerySessionService sessionService,
        ILogger<QueryResultsViewComponent> logger)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IViewComponentResult Invoke(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId)) return Content(string.Empty);

        var state = _sessionService.GetState(sessionId);

        if (state.Results == null || state.SelectedEntityType == null)
            return Content(
                "<p class=\"nqb-info-message\">No results yet. Click 'Run Query' to execute your query.</p>");

        // Get selected properties or all properties
        var selectedProperties = state.SelectedPropertyPaths.Any()
            ? state.SelectedPropertyPaths
            : state.Query?.SelectPropertyPaths.Select(p => p.Property.PropertyFullName).ToList() ?? new List<string>();

        if (!selectedProperties.Any())
            return Content(
                "<p class=\"nqb-warning-message\">No properties selected. Please select at least one property to display.</p>");

        // Extract items from QueryResult using reflection
        var resultsType = state.Results.GetType();
        var itemsProperty = resultsType.GetProperty("Items");
        var items = itemsProperty?.GetValue(state.Results) as IEnumerable;

        if (items == null) return Content("<p class=\"nqb-info-message\">No results found.</p>");

        var model = new QueryResultsViewModel
        {
            Items = items.Cast<object>().ToList(),
            SelectedProperties = selectedProperties,
            EntityType = state.SelectedEntityType
        };

        return View(model);
    }
}

/// <summary>
///     View model for query results
/// </summary>
public class QueryResultsViewModel
{
    public List<object> Items { get; set; } = new();
    public List<string> SelectedProperties { get; set; } = new();
    public Type? EntityType { get; set; }

    /// <summary>
    ///     Gets the value of a property from an object using dot notation
    /// </summary>
    public string GetPropertyValue(object item, string propertyPath)
    {
        if (item == null || string.IsNullOrEmpty(propertyPath))
            return string.Empty;

        try
        {
            var parts = propertyPath.Split('.');
            var current = item;

            foreach (var part in parts)
            {
                if (current == null)
                    return string.Empty;

                var property = current.GetType().GetProperty(part);
                if (property == null)
                    return string.Empty;

                current = property.GetValue(current);
            }

            return current?.ToString() ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}