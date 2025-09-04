using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
    private IQuery _query = null!;
    private QueryResultTable<TEntity> _queryResult;
    private IReadOnlyCollection<SelectPropertyPath> _selectedPropertyPaths = new List<SelectPropertyPath>();
    private int _totalItems;

    [Inject] private IQueryConfigurator QueryConfigurator { get; set; } = null!;
    [Parameter] public string Expression { get; set; } = string.Empty;

    public ValueTask DisposeAsync()
    {
        _query.OnChanged -= OnConditionConditionChanged;
        return ValueTask.CompletedTask;
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
        Expression = _query.Compile()?.ToString() ?? string.Empty;
        StateHasChanged();
    }

    private void OnPageStateChanged(object? sender, EventArgs e)
    {
        _ = RunQuery();
    }

    private async Task RunQuery()
    {
        _queryResult.Results = await _query.Execute<TEntity>(10);
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
        StateHasChanged();
    }
}