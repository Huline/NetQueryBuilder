using System.Collections.Concurrent;
using System.Text;
using Microsoft.AspNetCore.Http;
using NetQueryBuilder.AspNetCore.Models;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.AspNetCore.Services;

/// <summary>
///     Default implementation of IQuerySessionService using in-memory storage
/// </summary>
public class QuerySessionService : IQuerySessionService
{
    private readonly NetQueryBuilderOptions _options;
    private readonly ConcurrentDictionary<string, QuerySessionState> _sessions = new();

    public QuerySessionService(NetQueryBuilderOptions options)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public string GetOrCreateSessionId(HttpContext context)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        const string SessionIdKey = "NetQueryBuilder_SessionId";

        // Try to get existing session ID
        if (context.Session.TryGetValue(SessionIdKey, out var sessionIdBytes))
        {
            var sessionId = Encoding.UTF8.GetString(sessionIdBytes);
            if (!string.IsNullOrEmpty(sessionId))
                return sessionId;
        }

        // Create new session ID
        var newSessionId = Guid.NewGuid().ToString();
        context.Session.Set(SessionIdKey, Encoding.UTF8.GetBytes(newSessionId));

        return newSessionId;
    }

    public QuerySessionState GetState(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        return _sessions.GetOrAdd(sessionId, _ => new QuerySessionState
        {
            SessionId = sessionId,
            PageSize = _options.DefaultPageSize
        });
    }

    public void UpdateState(string sessionId, Action<QuerySessionState> update)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        if (update == null)
            throw new ArgumentNullException(nameof(update));

        var state = GetState(sessionId);
        update(state);
    }

    public void ClearSession(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        _sessions.TryRemove(sessionId, out _);
    }

    public IQuery GetOrCreateQuery(string sessionId, Type entityType, IQueryConfigurator configurator)
    {
        if (string.IsNullOrEmpty(sessionId))
            throw new ArgumentNullException(nameof(sessionId));

        if (entityType == null)
            throw new ArgumentNullException(nameof(entityType));

        if (configurator == null)
            throw new ArgumentNullException(nameof(configurator));

        var state = GetState(sessionId);
        var oldQuery = state.Query;
        var isSameEntityType = state.SelectedEntityType == entityType;

        // Always create a new query to get a fresh DbContext
        // The DbContext from previous requests will be disposed
        var query = configurator.BuildFor(entityType);

        // Subscribe to query events
        SubscribeToQueryEvents(query, sessionId);

        // If same entity type, transfer state from old query
        if (isSameEntityType && oldQuery != null)
        {
            // Transfer conditions from old query to new query
            TransferConditions(oldQuery.Condition, query.Condition, query.ConditionPropertyPaths);

            // Transfer selected properties
            foreach (var oldSelect in oldQuery.SelectPropertyPaths.Where(p => p.IsSelected))
            {
                var newSelect = query.SelectPropertyPaths
                    .FirstOrDefault(p => p.Property.PropertyFullName == oldSelect.Property.PropertyFullName);
                if (newSelect != null)
                    newSelect.IsSelected = true;
            }
        }
        else
        {
            // Entity type changed, clear selections
            state.SelectedPropertyPaths.Clear();
            state.Results = null;
            state.CurrentPage = 1;
        }

        // Update state
        state.Query = query;
        state.SelectedEntityType = entityType;
        state.CurrentExpression = query.ToString();

        return query;
    }

    private static void TransferConditions(
        BlockCondition sourceBlock,
        BlockCondition targetBlock,
        IReadOnlyCollection<PropertyPath> availableProperties)
    {
        // Transfer logical operator
        targetBlock.LogicalOperator = sourceBlock.LogicalOperator;

        // Transfer each condition
        foreach (var condition in sourceBlock.Conditions)
        {
            if (condition is SimpleCondition simpleCondition)
            {
                // Find matching property in new query by name
                var matchingProperty = availableProperties
                    .FirstOrDefault(p => p.PropertyFullName == simpleCondition.PropertyPath.PropertyFullName);

                if (matchingProperty != null)
                {
                    var newCondition = targetBlock.CreateNew(matchingProperty);
                    newCondition.LogicalOperator = simpleCondition.LogicalOperator;
                    newCondition.Value = simpleCondition.Value;

                    // Try to find matching operator by name
                    var matchingOperator = matchingProperty.GetCompatibleOperators()
                        .FirstOrDefault(op => op.GetType() == simpleCondition.Operator.GetType());
                    if (matchingOperator != null)
                        newCondition.Operator = matchingOperator;
                }
            }
            else if (condition is BlockCondition nestedBlock)
            {
                // Create a new nested block and recursively transfer
                var newNestedBlock = new BlockCondition(
                    new List<ICondition>(),
                    nestedBlock.LogicalOperator,
                    targetBlock);
                targetBlock.Add(newNestedBlock);
                TransferConditions(nestedBlock, newNestedBlock, availableProperties);
            }
        }
    }

    public void UpdateQueryExpression(string sessionId)
    {
        var state = GetState(sessionId);
        if (state.Query != null) state.CurrentExpression = state.Query.ToString();
    }

    public void ToggleProperty(string sessionId, string propertyPath)
    {
        if (string.IsNullOrEmpty(propertyPath))
            throw new ArgumentNullException(nameof(propertyPath));

        var state = GetState(sessionId);

        if (state.Query != null)
        {
            // Find the SelectPropertyPath and toggle its IsSelected property
            var selectProperty = state.Query.SelectPropertyPaths
                .FirstOrDefault(p => p.Property.PropertyFullName == propertyPath);

            if (selectProperty != null)
            {
                selectProperty.IsSelected = !selectProperty.IsSelected;

                // Update the selected property paths list in state
                if (selectProperty.IsSelected)
                {
                    if (!state.SelectedPropertyPaths.Contains(propertyPath))
                        state.SelectedPropertyPaths.Add(propertyPath);
                }
                else
                {
                    state.SelectedPropertyPaths.Remove(propertyPath);
                }
            }
        }
    }

    public List<string> GetSelectedProperties(string sessionId)
    {
        var state = GetState(sessionId);
        return state.SelectedPropertyPaths;
    }

    public void SaveResults<T>(string sessionId, QueryResult<T> results)
    {
        if (results == null)
            throw new ArgumentNullException(nameof(results));

        var state = GetState(sessionId);
        state.Results = results;
        state.TotalPages = results.TotalPage;
        state.TotalItems = results.TotalItems;
        state.CurrentPage = results.CurrentPage;
    }

    public QueryResult<T>? GetResults<T>(string sessionId)
    {
        var state = GetState(sessionId);
        return state.Results as QueryResult<T>;
    }

    public void SetPage(string sessionId, int page)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page), "Page number must be greater than 0");

        var state = GetState(sessionId);
        state.CurrentPage = page;
    }

    public List<Type> GetAvailableEntityTypes(IQueryConfigurator configurator)
    {
        if (configurator == null)
            throw new ArgumentNullException(nameof(configurator));

        // Get all entity types from the configurator
        var entities = configurator.GetEntities();
        return entities.ToList();
    }

    private void SubscribeToQueryEvents(IQuery query, string sessionId)
    {
        // Subscribe to condition changes
        query.OnChanged += (sender, args) =>
        {
            UpdateState(sessionId, state => { state.CurrentExpression = query.ToString(); });
        };

        // Subscribe to selection changes (if the event exists)
        // Note: The core library might not have OnSelectionChanged yet
        // This is prepared for future enhancements
    }
}