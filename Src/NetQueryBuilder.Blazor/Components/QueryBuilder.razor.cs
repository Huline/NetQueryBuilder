using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
    private List<TEntity> _data = new();
    private IEnumerable<PropertyPath> _propertyPaths = Enumerable.Empty<PropertyPath>();
    private IQuery<TEntity> _query = null!;
    private IEnumerable<PropertyPath> _selectedPropertyPaths = new List<PropertyPath>();
    [Parameter] public required string Expression { get; set; }
    private BlockCondition Condition { get; set; } = null!;

    public ValueTask DisposeAsync()
    {
        Condition.ConditionChanged -= OnConditionConditionChanged;
        return ValueTask.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        _query = QueryFactory.Create<TEntity>();
        _propertyPaths = _query.AvailableProperties();
        _selectedPropertyPaths = _propertyPaths.ToList();

        Condition = _query.Condition;
        Condition.ConditionChanged += OnConditionConditionChanged;
        await base.OnInitializedAsync();
    }

    private void OnConditionConditionChanged(object? sender, EventArgs args)
    {
        OnChanged(Condition.Compile());
    }

    private async Task RunQuery()
    {
        var data = await _query.Execute(_selectedPropertyPaths);

        _data = (data as IEnumerable<TEntity>)?.ToList() ?? new List<TEntity>();
    }

    private void OnChanged(Expression body)
    {
        _query.Compile();
        StateHasChanged();
    }

    private object? GetNestedPropertyValue(object entity, string propertyPath)
    {
        var segments = propertyPath.Split('.');
        var currentObject = entity;

        foreach (var segment in segments)
        {
            // Récupérer la propriété par réflexion
            if (currentObject == null) break;

            var propInfo = currentObject.GetType().GetProperty(segment);
            currentObject = propInfo?.GetValue(currentObject);
        }

        return currentObject;
    }
}