using System.Collections;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Blazor.ExpressionVisitors.Extensions;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Services;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
      [Parameter] public string Expression { get; set; }

    private List<TEntity> _data = new();
    private IEnumerable<PropertyPath> propertyPaths = Enumerable.Empty<PropertyPath>();
    private IEnumerable<PropertyPath> SelectedPropertyPaths = new List<PropertyPath>();
    private IQuery<TEntity> _query;
    private BlockCondition Condition { get; set; }
    protected override async Task OnInitializedAsync()
    {
        _query = QueryFactory.Create<TEntity>();
        propertyPaths = _query.AvailableProperties();
        SelectedPropertyPaths = propertyPaths.ToList();

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
        var data = await _query.Execute(SelectedPropertyPaths);
        
        _data = (data as IEnumerable<TEntity>)?.ToList();
    }

    private void OnChanged(Expression body)
    {
        _query.Compile();
        StateHasChanged();
    }

    private object GetNestedPropertyValue(object entity, string propertyPath)
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

    public ValueTask DisposeAsync()
    {
        Condition.ConditionChanged -= OnConditionConditionChanged;
        return ValueTask.CompletedTask;
    }

}