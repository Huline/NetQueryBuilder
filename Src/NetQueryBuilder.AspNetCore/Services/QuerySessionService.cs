using NetQueryBuilder.AspNetCore.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Concurrent;

namespace NetQueryBuilder.AspNetCore.Services;

/// <summary>
/// Default implementation of IQuerySessionService using in-memory storage
/// </summary>
public class QuerySessionService : IQuerySessionService
{
    private readonly ConcurrentDictionary<string, QuerySessionState> _sessions = new();
    private readonly NetQueryBuilderOptions _options;

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
            var sessionId = System.Text.Encoding.UTF8.GetString(sessionIdBytes);
            if (!string.IsNullOrEmpty(sessionId))
                return sessionId;
        }

        // Create new session ID
        var newSessionId = Guid.NewGuid().ToString();
        context.Session.Set(SessionIdKey, System.Text.Encoding.UTF8.GetBytes(newSessionId));

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

        // If query exists and entity type matches, return it
        if (state.Query != null && state.SelectedEntityType == entityType)
        {
            return state.Query;
        }

        // Create new query
        var query = configurator.BuildFor(entityType);

        // Subscribe to query events
        SubscribeToQueryEvents(query, sessionId);

        // Update state
        state.Query = query;
        state.SelectedEntityType = entityType;
        state.SelectedPropertyPaths.Clear();
        state.Results = null;
        state.CurrentPage = 1;
        state.CurrentExpression = query.Stringify();

        return query;
    }

    public void UpdateQueryExpression(string sessionId)
    {
        var state = GetState(sessionId);
        if (state.Query != null)
        {
            state.CurrentExpression = state.Query.Stringify();
        }
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
        state.TotalPages = results.TotalPages;
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
        query.OnConditionChanged += (sender, args) =>
        {
            UpdateState(sessionId, state =>
            {
                state.CurrentExpression = query.Stringify();
            });
        };

        // Subscribe to selection changes (if the event exists)
        // Note: The core library might not have OnSelectionChanged yet
        // This is prepared for future enhancements
    }
}
