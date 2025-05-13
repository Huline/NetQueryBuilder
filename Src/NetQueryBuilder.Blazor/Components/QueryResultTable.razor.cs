using Microsoft.AspNetCore.Components;
using MudBlazor;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryResultTable<TEntity> : ComponentBase
{
    [Parameter] public QueryResult<TEntity>? Results { get; set; }
    [Parameter] [EditorRequired] public IReadOnlyCollection<SelectPropertyPath> Properties { get; set; } = new List<SelectPropertyPath>();
    [Parameter] public int TotalItems { get; set; }

    [Parameter] public bool Bordered { get; set; } = true;
    [Parameter] public bool Striped { get; set; } = true;
    [Parameter] public bool Loading { get; set; }
    [Parameter] public Color LoadingColor { get; set; } = Color.Primary;
    [Parameter] public bool Hover { get; set; } = true;
    [Parameter] public bool Dense { get; set; }


    private IEnumerable<SelectPropertyPath> SelectedProperties => Properties.Where(p => p.IsSelected);

    public void Dispose()
    {
    }


    protected override void OnInitialized()
    {
        base.OnInitialized();
    }

    private void OnPageStateChanged(object? sender, EventArgs e)
    {
        StateHasChanged();
    }
}