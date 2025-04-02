using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
    private List<TEntity> _data = new();
    private IQuery _query = null!;
    private IReadOnlyCollection<SelectPropertyPath> _selectedPropertyPaths = new List<SelectPropertyPath>();
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
        _query.Condition.CreateNew(_query.ConditionPropertyPaths.First());
        _selectedPropertyPaths = _query.SelectPropertyPaths;
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
        var data = await _query.Execute();

        _data = (data as IEnumerable<TEntity>)?.ToList() ?? new List<TEntity>();
    }
    private void OnPropertySelectionChanged(dynamic property, ChangeEventArgs e)
    {
        property.IsSelected = (bool)e.Value;
        StateHasChanged();
    }

}