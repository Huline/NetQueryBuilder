using NetQueryBuilder.AspNetCore.Models;
using Microsoft.AspNetCore.Http;

namespace NetQueryBuilder.AspNetCore.Services;

/// <summary>
/// Service for managing query building session state
/// </summary>
public interface IQuerySessionService
{
    /// <summary>
    /// Gets or creates a session ID from the HTTP context
    /// </summary>
    string GetOrCreateSessionId(HttpContext context);

    /// <summary>
    /// Gets the current session state
    /// </summary>
    QuerySessionState GetState(string sessionId);

    /// <summary>
    /// Updates the session state
    /// </summary>
    void UpdateState(string sessionId, Action<QuerySessionState> update);

    /// <summary>
    /// Clears all session data
    /// </summary>
    void ClearSession(string sessionId);

    /// <summary>
    /// Gets or creates a query for the specified entity type
    /// </summary>
    IQuery GetOrCreateQuery(string sessionId, Type entityType, IQueryConfigurator configurator);

    /// <summary>
    /// Updates the query expression string
    /// </summary>
    void UpdateQueryExpression(string sessionId);

    /// <summary>
    /// Toggles a property selection
    /// </summary>
    void ToggleProperty(string sessionId, string propertyPath);

    /// <summary>
    /// Gets the list of selected properties
    /// </summary>
    List<string> GetSelectedProperties(string sessionId);

    /// <summary>
    /// Saves query results
    /// </summary>
    void SaveResults<T>(string sessionId, QueryResult<T> results);

    /// <summary>
    /// Gets query results
    /// </summary>
    QueryResult<T>? GetResults<T>(string sessionId);

    /// <summary>
    /// Sets the current page number
    /// </summary>
    void SetPage(string sessionId, int page);

    /// <summary>
    /// Gets available entity types from the configurator
    /// </summary>
    List<Type> GetAvailableEntityTypes(IQueryConfigurator configurator);
}
