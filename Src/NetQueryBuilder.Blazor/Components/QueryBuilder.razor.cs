using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
    private List<TEntity> _data = new();
    private IEnumerable<PropertyPath> _propertyPaths = Enumerable.Empty<PropertyPath>();
    private IQuery _query = null!;
    private IEnumerable<PropertyPath> _selectedPropertyPaths = new List<PropertyPath>();
    [Inject] private IQueryConfigurator QueryConfigurator { get; set; } = null!;
    [Parameter] public required string Expression { get; set; }

    public ValueTask DisposeAsync()
    {
        _query.OnChanged -= OnConditionConditionChanged;
        return ValueTask.CompletedTask;
    }

    protected override async Task OnInitializedAsync()
    {
        _query = QueryConfigurator.BuildFor<TEntity>();
        _propertyPaths = _query.AvailableProperties();
        _selectedPropertyPaths = _propertyPaths.ToList();
        _query.OnChanged += OnConditionConditionChanged;
        await base.OnInitializedAsync();
    }

    private void OnConditionConditionChanged(object? sender, EventArgs args)
    {
        Expression = _query.Compile().ToString();
        StateHasChanged();
    }

    private async Task RunQuery()
    {
        var data = await _query.Execute(_selectedPropertyPaths);

        _data = (data as IEnumerable<TEntity>)?.ToList() ?? new List<TEntity>();
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