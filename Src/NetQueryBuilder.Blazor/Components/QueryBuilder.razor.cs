using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilder<TEntity> : IAsyncDisposable
{
    private List<TEntity> _data = new();
    private IQuery _query = null!;
    private QueryResultTable<TEntity> _queryResult = null!;
    private IReadOnlyCollection<SelectPropertyPath> _selectedPropertyPaths = new List<SelectPropertyPath>();
    private int _totalItems;

    [Inject] private IQueryConfigurator QueryConfigurator { get; set; } = null!;
    [Parameter] public string Expression { get; set; } = string.Empty;
    public List<PropertyPath> SelectedProperty => _selectedPropertyPaths.Select(s => s.Property).ToList();

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

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender) _queryResult.PageState.StateChanged += OnPageStateChanged;
        base.OnAfterRender(firstRender);
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
        // Obtenir l'état de pagination actuel
        var pageSize = _queryResult.PageState.PageSize;
        var currentPage = _queryResult.PageState.CurrentPage;
        var offset = currentPage * pageSize;

        // Exécuter la requête avec pagination
        var result = await _query.Execute<TEntity>(pageSize, offset);

        // Mettre à jour les données affichées
        _data = result?.ToList() ?? new List<TEntity>();

        // Mettre à jour le nombre total d'éléments (requête distincte)
        var countResult = await _query.Execute();
        _totalItems = countResult?.Count() ?? 0;

        // Forcer la mise à jour de l'interface
        StateHasChanged();
    }

    private void SelectedValuesChanged(IEnumerable<SelectPropertyPath?>? obj)
    {
        if (obj == null) return;
        var selectPropertyPaths = obj.ToHashSet();
        foreach (var selectedPropertyPath in _selectedPropertyPaths) selectedPropertyPath.IsSelected = selectPropertyPaths.Contains(selectedPropertyPath);
    }
}