using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using NetQueryBuilder.AspNetCore.Models;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.AspNetCore.Pages;

/// <summary>
///     Base page model for pages using the NetQueryBuilder
///     Provides common functionality for query building and execution
/// </summary>
public abstract class NetQueryPageModelBase : PageModel
{
    protected readonly IQueryConfigurator Configurator;
    protected readonly IQuerySessionService SessionService;

    protected NetQueryPageModelBase(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator)
    {
        SessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        Configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
    }

    /// <summary>
    ///     Gets the current session ID
    /// </summary>
    public string SessionId => SessionService.GetOrCreateSessionId(HttpContext);

    /// <summary>
    ///     Gets the current session state
    /// </summary>
    public QuerySessionState State => SessionService.GetState(SessionId);

    /// <summary>
    ///     Selected entity type (for binding)
    /// </summary>
    [BindProperty]
    public string? SelectedEntityType { get; set; }

    /// <summary>
    ///     Selected properties (for binding)
    /// </summary>
    [BindProperty]
    public List<string> SelectedProperties { get; set; } = new();

    /// <summary>
    ///     Condition values from form (for binding)
    /// </summary>
    [BindProperty]
    public List<ConditionViewModel> Conditions { get; set; } = new();

    /// <summary>
    ///     Handler for changing the entity type
    /// </summary>
    public virtual IActionResult OnPostChangeEntity()
    {
        if (!string.IsNullOrEmpty(SelectedEntityType))
        {
            // Find the type from the configurator's known entities
            // Type.GetType() doesn't work for types in other assemblies without assembly-qualified names
            var type = Configurator.GetEntities()
                .FirstOrDefault(t => t.FullName == SelectedEntityType || t.Name == SelectedEntityType);

            if (type != null) SessionService.GetOrCreateQuery(SessionId, type, Configurator);
        }

        return Page();
    }

    /// <summary>
    ///     Handler for toggling property selection
    /// </summary>
    public virtual IActionResult OnPostToggleProperty(string propertyPath)
    {
        if (!string.IsNullOrEmpty(propertyPath)) SessionService.ToggleProperty(SessionId, propertyPath);
        return Page();
    }

    /// <summary>
    ///     Handler for updating property selections (batch update)
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
            SessionService.UpdateState(SessionId,
                state => { state.SelectedPropertyPaths = SelectedProperties.ToList(); });
        }

        return Page();
    }

    /// <summary>
    ///     Handler for adding a new condition
    /// </summary>
    /// <param name="propertyPath">Optional property path to use for the new condition. If not specified, uses the first available property.</param>
    /// <param name="parentId">Optional parent ID for nested conditions</param>
    public virtual IActionResult OnPostAddCondition(string? propertyPath = null, string? parentId = null)
    {
        var query = State.Query;
        if (query != null && query.ConditionPropertyPaths.Any())
        {
            // Find the specified property or use the first available
            var property = !string.IsNullOrEmpty(propertyPath)
                ? query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName == propertyPath)
                : null;

            // Fallback to first property if not found
            property ??= query.ConditionPropertyPaths.First();

            // Add a new simple condition to the root block
            query.Condition.CreateNew(property);
        }

        return Page();
    }

    /// <summary>
    ///     Handler for removing a condition
    /// </summary>
    public virtual IActionResult OnPostRemoveCondition(int index)
    {
        var query = State.Query;
        if (query != null && index >= 0 && index < query.Condition.Conditions.Count) query.Condition.RemoveAt(index);
        return Page();
    }

    /// <summary>
    ///     Handler for adding a condition group
    /// </summary>
    public virtual IActionResult OnPostAddGroup(string? parentId = null)
    {
        var query = State.Query;
        if (query != null)
            // Add a new block condition (group)
            query.Condition.CreateNew();
        return Page();
    }

    /// <summary>
    ///     Handler for ungrouping conditions
    /// </summary>
    public virtual IActionResult OnPostUngroup(int index)
    {
        var query = State.Query;
        if (query != null && index >= 0 && index < query.Condition.Conditions.Count)
            // Get the block condition at the index
            if (query.Condition[index] is BlockCondition block)
                block.Ungroup(block.Conditions);

        return Page();
    }

    /// <summary>
    ///     Handler for executing the query.
    ///     Uses reflection to automatically dispatch to the correct generic ExecuteQueryAsync method
    ///     based on the selected entity type.
    /// </summary>
    public virtual async Task<IActionResult> OnPostExecuteQueryAsync()
    {
        await ExecuteQueryForCurrentEntityAsync();
        return Page();
    }

    /// <summary>
    ///     Executes the query for the currently selected entity type using reflection.
    ///     This eliminates the need for if-else chains based on entity type in derived classes.
    /// </summary>
    /// <returns>The query result as an object, or null if no entity type is selected</returns>
    protected async Task<object?> ExecuteQueryForCurrentEntityAsync()
    {
        var entityType = State.SelectedEntityType;
        if (entityType == null)
            return null;

        // Get the generic ExecuteQueryAsync<T> method and make it specific to the entity type
        var method = typeof(NetQueryPageModelBase)
            .GetMethod(nameof(ExecuteQueryAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
            throw new InvalidOperationException("ExecuteQueryAsync method not found.");

        var genericMethod = method.MakeGenericMethod(entityType);

        // Invoke the method and await the result
        var task = (Task?)genericMethod.Invoke(this, Array.Empty<object>());
        if (task == null)
            return null;

        await task.ConfigureAwait(false);

        // Get the result from the Task<QueryResult<T>>
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }

    /// <summary>
    ///     Navigates to the specified page for the current entity type using reflection.
    ///     This eliminates the need for if-else chains based on entity type in derived classes.
    /// </summary>
    /// <param name="page">The page number to navigate to</param>
    /// <returns>The query result as an object, or null if no entity type is selected</returns>
    protected async Task<object?> GoToPageForCurrentEntityAsync(int page)
    {
        var entityType = State.SelectedEntityType;
        if (entityType == null)
            return null;

        // Get the generic GoToPageAsync<T> method and make it specific to the entity type
        var method = typeof(NetQueryPageModelBase)
            .GetMethod(nameof(GoToPageAsync), BindingFlags.NonPublic | BindingFlags.Instance);

        if (method == null)
            throw new InvalidOperationException("GoToPageAsync method not found.");

        var genericMethod = method.MakeGenericMethod(entityType);

        // Invoke the method and await the result
        var task = (Task?)genericMethod.Invoke(this, new object[] { page });
        if (task == null)
            return null;

        await task.ConfigureAwait(false);

        // Get the result from the Task<QueryResult<T>>
        var resultProperty = task.GetType().GetProperty("Result");
        return resultProperty?.GetValue(task);
    }

    /// <summary>
    ///     Handler for changing the page
    /// </summary>
    public virtual async Task<IActionResult> OnPostChangePageAsync(int page)
    {
        if (page < 1)
            return Page();

        var state = State;
        if (state.Results == null)
            return Page();

        // Navigate to the specified page using reflection-based dispatch
        await GoToPageForCurrentEntityAsync(page);

        return Page();
    }

    /// <summary>
    ///     Handler for clearing the query
    /// </summary>
    public virtual IActionResult OnPostClearQuery()
    {
        SessionService.ClearSession(SessionId);
        return RedirectToPage();
    }

    /// <summary>
    ///     Helper method to execute query for a specific entity type
    /// </summary>
    protected async Task<QueryResult<T>?> ExecuteQueryAsync<T>()
    {
        var entityType = State.SelectedEntityType;
        if (entityType == null)
            return null;

        try
        {
            // Apply form values to the current query's conditions before refreshing
            ApplyConditionValuesFromForm();

            // Always get a fresh query with current DbContext
            // This transfers conditions from the old query to a new one
            var query = SessionService.GetOrCreateQuery(SessionId, entityType, Configurator);

            var result = await query.Execute<T>(State.PageSize);
            SessionService.SaveResults(SessionId, result);
            return result;
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error executing query: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    ///     Applies condition values from form binding to the current query
    /// </summary>
    protected void ApplyConditionValuesFromForm()
    {
        var query = State.Query;
        if (query == null || Conditions == null || !Conditions.Any())
            return;

        var simpleConditions = query.Condition.Conditions.OfType<SimpleCondition>().ToList();

        for (int i = 0; i < Conditions.Count && i < simpleConditions.Count; i++)
        {
            var formCondition = Conditions[i];
            var queryCondition = simpleConditions[i];

            // Update property if changed
            if (!string.IsNullOrEmpty(formCondition.PropertyPath) &&
                formCondition.PropertyPath != queryCondition.PropertyPath.PropertyFullName)
            {
                var newProperty = query.ConditionPropertyPaths
                    .FirstOrDefault(p => p.PropertyFullName == formCondition.PropertyPath);
                if (newProperty != null)
                {
                    queryCondition.PropertyPath = newProperty;
                }
            }

            // Update operator if changed
            if (!string.IsNullOrEmpty(formCondition.Operator))
            {
                var compatibleOperators = queryCondition.PropertyPath.GetCompatibleOperators();
                var newOperator = compatibleOperators
                    .FirstOrDefault(op => op.GetType().Name == formCondition.Operator);
                if (newOperator != null && newOperator.GetType() != queryCondition.Operator.GetType())
                {
                    queryCondition.Operator = newOperator;
                }
            }

            // Update the value if provided
            if (formCondition.Value != null)
            {
                // Convert the string value to the appropriate type
                var targetType = queryCondition.PropertyPath.PropertyType;
                var convertedValue = ConvertValue(formCondition.Value, targetType);
                if (convertedValue != null)
                {
                    queryCondition.Value = convertedValue;
                }
            }
        }
    }

    /// <summary>
    ///     Converts a string value to the target type
    /// </summary>
    private static object? ConvertValue(string value, Type targetType)
    {
        try
        {
            // Handle nullable types
            var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

            if (string.IsNullOrEmpty(value))
                return underlyingType.IsValueType ? Activator.CreateInstance(underlyingType) : null;

            if (underlyingType == typeof(string))
                return value;

            if (underlyingType == typeof(int))
                return int.Parse(value);

            if (underlyingType == typeof(long))
                return long.Parse(value);

            if (underlyingType == typeof(decimal))
                return decimal.Parse(value);

            if (underlyingType == typeof(double))
                return double.Parse(value);

            if (underlyingType == typeof(float))
                return float.Parse(value);

            if (underlyingType == typeof(bool))
                return bool.Parse(value);

            if (underlyingType == typeof(DateTime))
                return DateTime.Parse(value);

            if (underlyingType == typeof(Guid))
                return Guid.Parse(value);

            if (underlyingType.IsEnum)
                return Enum.Parse(underlyingType, value);

            // Fallback: try Convert.ChangeType
            return Convert.ChangeType(value, underlyingType);
        }
        catch
        {
            // If conversion fails, return null
            return null;
        }
    }

    /// <summary>
    ///     Helper method to navigate to a specific page
    /// </summary>
    protected async Task<QueryResult<T>?> GoToPageAsync<T>(int page)
    {
        var entityType = State.SelectedEntityType;
        if (entityType == null)
            return null;

        try
        {
            // Get a fresh query with current DbContext and re-execute for the new page
            // We can't use the stored QueryResult.GoToPage because it holds a reference
            // to a delegate that uses the old (disposed) DbContext
            var query = SessionService.GetOrCreateQuery(SessionId, entityType, Configurator);

            // Update page in state
            SessionService.SetPage(SessionId, page);

            // Execute query to get fresh results
            var result = await query.Execute<T>(State.PageSize);

            // If we need a different page, use GoToPage on the fresh result
            // This works because the delegate was just created with the current DbContext
            if (page > 1 && page <= result.TotalPage)
            {
                result = await result.GoToPage(page - 1); // GoToPage is 0-indexed
            }

            SessionService.SaveResults(SessionId, result);
            return result;
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Error navigating to page: {ex.Message}");
            return null;
        }
    }
}