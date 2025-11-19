using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
    private IQuery _query = null!;
    private QueryResultTable<TEntity> _queryResult = null!;
    private IReadOnlyCollection<SelectPropertyPath> _selectedPropertyPaths = new List<SelectPropertyPath>();
    private int _totalItems;
    private QueryResult<TEntity>? _queryResults;
    private System.Threading.Timer? _debounceTimer;
    private string _previousExpression = string.Empty;

    [Inject] private IQueryConfigurator QueryConfigurator { get; set; } = null!;
    [Parameter] public string Expression { get; set; } = string.Empty;
    [Parameter] public int DebounceMilliseconds { get; set; } = 300;

    public ValueTask DisposeAsync()
    {
        _query.OnChanged -= OnConditionConditionChanged;
        _debounceTimer?.Dispose();
        return ValueTask.CompletedTask;
    }

    protected override bool ShouldRender()
    {
        // Only re-render if expression or results actually changed
        return Expression != _previousExpression || _queryResults != null;
    }

    protected override async Task OnInitializedAsync()
    {
        _query = QueryConfigurator.BuildFor<TEntity>();
        _query.Condition.CreateNew(_query.ConditionPropertyPaths.First());
        _selectedPropertyPaths = _query.SelectPropertyPaths;
        _query.OnChanged += OnConditionConditionChanged;
        await base.OnInitializedAsync();
    }


    private void OnConditionConditionChanged(object? sender, EventArgs args)
    {
        // Debounce expression updates to avoid excessive re-renders
        _debounceTimer?.Dispose();
        _debounceTimer = new System.Threading.Timer(_ =>
        {
            InvokeAsync(() =>
            {
                var newExpression = _query.Compile()?.ToString() ?? string.Empty;
                if (Expression != newExpression)
                {
                    _previousExpression = Expression;
                    Expression = newExpression;
                    StateHasChanged();
                }
            });
        }, null, DebounceMilliseconds, Timeout.Infinite);
    }

    private void OnPageStateChanged(object? sender, EventArgs e)
    {
        _ = RunQuery();
    }

    private async Task RunQuery()
    {
        var queryResult = await _query.Execute<TEntity>(10);
        _queryResults = queryResult;
        _totalItems = queryResult.TotalItems;
        StateHasChanged();
    }

    private void SelectedValuesChanged(IEnumerable<SelectPropertyPath?>? obj)
    {
        if (obj == null) return;
        var selectPropertyPaths = obj.ToHashSet();
        foreach (var selectedPropertyPath in _selectedPropertyPaths) selectedPropertyPath.IsSelected = selectPropertyPaths.Contains(selectedPropertyPath);
    }
    
    private void TogglePropertySelection(SelectPropertyPath property, bool isSelected)
    {
        property.IsSelected = isSelected;
        // No StateHasChanged needed - Blazor will re-render automatically
    }

    private void SelectAllFields()
    {
        foreach (var property in _selectedPropertyPaths)
        {
            property.IsSelected = true;
        }
        // Only call StateHasChanged once after all updates
        StateHasChanged();
    }

    private void DeselectAllFields()
    {
        foreach (var property in _selectedPropertyPaths)
        {
            property.IsSelected = false;
        }
        // Only call StateHasChanged once after all updates
        StateHasChanged();
    }
    
    private string GetFieldTypeDisplay(SelectPropertyPath propertyPath)
    {
        var propertyType = propertyPath.Property.PropertyType;
        return propertyType.Name switch
        {
            "String" => "Text",
            "Int32" => "Number",
            "Int64" => "Number",
            "Double" => "Decimal",
            "Decimal" => "Decimal",
            "DateTime" => "Date/Time",
            "Boolean" => "Yes/No",
            _ => propertyType.Name
        };
    }
}