using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetQueryBuilder.AspNetCore.Models;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.AspNetCore.Pages;

/// <summary>
/// Base page model for pages using the NetQueryBuilder
/// Provides common functionality for query building and execution
/// </summary>
public abstract class NetQueryPageModelBase : PageModel
{
    protected readonly IQuerySessionService SessionService;
    protected readonly IQueryConfigurator Configurator;

    protected NetQueryPageModelBase(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator)
    {
        SessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        Configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
    }

    /// <summary>
    /// Gets the current session ID
    /// </summary>
    public string SessionId => SessionService.GetOrCreateSessionId(HttpContext);

    /// <summary>
    /// Gets the current session state
    /// </summary>
    public QuerySessionState State => SessionService.GetState(SessionId);

    /// <summary>
    /// Selected entity type (for binding)
    /// </summary>
    [BindProperty]
    public string? SelectedEntityType { get; set; }

    /// <summary>
    /// Selected properties (for binding)
    /// </summary>
    [BindProperty]
    public List<string> SelectedProperties { get; set; } = new();

    /// <summary>
    /// Handler for changing the entity type
    /// </summary>
    public virtual IActionResult OnPostChangeEntity()
    {
        if (!string.IsNullOrEmpty(SelectedEntityType))
        {
            var type = Type.GetType(SelectedEntityType);
            if (type != null)
            {
                SessionService.GetOrCreateQuery(SessionId, type, Configurator);
            }
        }
        return Page();
    }

    /// <summary>
    /// Handler for toggling property selection
    /// </summary>
    public virtual IActionResult OnPostToggleProperty(string propertyPath)
    {
        if (!string.IsNullOrEmpty(propertyPath))
        {
            SessionService.ToggleProperty(SessionId, propertyPath);
        }
        return Page();
    }

    /// <summary>
    /// Handler for updating property selections (batch update)
    /// </summary>
    public virtual IActionResult OnPostUpdateProperties()
    {
        var query = State.Query;
        if (query != null)
        {
            // Update all SelectPropertyPath objects based on SelectedProperties list
            foreach (var selectProperty in query.SelectPropertyPaths)
            {
                var propertyPath = selectProperty.Property.PropertyFullName;
                selectProperty.IsSelected = SelectedProperties.Contains(propertyPath);
            }

            // Update session state
            SessionService.UpdateState(SessionId, state =>
            {
                state.SelectedPropertyPaths = SelectedProperties.ToList();
            });
        }
        return Page();
    }

    /// <summary>
    /// Handler for adding a new condition
    /// </summary>
    public virtual IActionResult OnPostAddCondition(string? parentId = null)
    {
        var query = State.Query;
        if (query != null)
        {
            // Add a new simple condition to the root block
            if (query.ConditionPropertyPaths.Any())
            {
                var firstProperty = query.ConditionPropertyPaths.First();
                query.Condition.CreateNew(firstProperty);
            }
        }
        return Page();
    }

    /// <summary>
    /// Handler for removing a condition
    /// </summary>
    public virtual IActionResult OnPostRemoveCondition(int index)
    {
        var query = State.Query;
        if (query != null && index >= 0 && index < query.Condition.Children.Count)
        {
            query.Condition.RemoveAt(index);
        }
        return Page();
    }

    /// <summary>
    /// Handler for adding a condition group
    /// </summary>
    public virtual IActionResult OnPostAddGroup(string? parentId = null)
    {
        var query = State.Query;
        if (query != null)
        {
            // Add a new block condition (group)
            query.Condition.CreateBlock();
        }
        return Page();
    }

    /// <summary>
    /// Handler for ungrouping conditions
    /// </summary>
    public virtual IActionResult OnPostUngroup(int index)
    {
        var query = State.Query;
        if (query != null && index >= 0 && index < query.Condition.Children.Count)
        {
            // Get the block condition at the index
            if (query.Condition.Children[index] is BlockCondition block)
            {
                // Move all children to parent and remove the block
                var children = block.Children.ToList();
                query.Condition.RemoveAt(index);

                foreach (var child in children)
                {
                    query.Condition.Add(child);
                }
            }
        }
        return Page();
    }

    /// <summary>
    /// Handler for executing the query (to be overridden by derived classes for type-specific execution)
    /// </summary>
    public virtual async Task<IActionResult> OnPostExecuteQueryAsync()
    {
        // This method should be overridden in derived classes to handle specific entity types
        // Because we need the generic type parameter at compile time
        await Task.CompletedTask;
        return Page();
    }

    /// <summary>
    /// Handler for changing the page
    /// </summary>
    public virtual async Task<IActionResult> OnPostChangePageAsync(int page)
    {
        if (page < 1)
            return Page();

        var state = State;
        if (state.Results == null)
            return Page();

        // Update the page in session
        SessionService.SetPage(SessionId, page);

        // Re-execute the query to get new page
        // This will be handled by the derived class's OnPostExecuteQueryAsync
        return await OnPostExecuteQueryAsync();
    }

    /// <summary>
    /// Handler for clearing the query
    /// </summary>
    public virtual IActionResult OnPostClearQuery()
    {
        SessionService.ClearSession(SessionId);
        return RedirectToPage();
    }

    /// <summary>
    /// Helper method to execute query for a specific entity type
    /// </summary>
    protected async Task<QueryResult<T>?> ExecuteQueryAsync<T>()
    {
        var query = State.Query;
        if (query == null)
            return null;

        try
        {
            var result = await query.Execute<T>(State.PageSize);
            SessionService.SaveResults(SessionId, result);
            return result;
        }
        catch (Exception ex)
        {
            // Log the error
            ModelState.AddModelError(string.Empty, $"Error executing query: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Helper method to navigate to a specific page
    /// </summary>
    protected async Task<QueryResult<T>?> GoToPageAsync<T>(int page)
    {
        var currentResults = SessionService.GetResults<T>(SessionId);
        if (currentResults == null)
            return null;

        try
        {
            var newResults = await currentResults.GoToPage(page - 1); // GoToPage is 0-indexed
            SessionService.SaveResults(SessionId, newResults);
            return newResults;
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error navigating to page: {ex.Message}");
            return null;
        }
    }
}
