using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryResultTable<TEntity> : ComponentBase
{
    [Parameter] [EditorRequired] public IReadOnlyCollection<TEntity> Data { get; set; } = new List<TEntity>();
    [Parameter] [EditorRequired] public IReadOnlyCollection<SelectPropertyPath> Properties { get; set; } = new List<SelectPropertyPath>();

    [Parameter] public bool Bordered { get; set; } = true;
    [Parameter] public bool Striped { get; set; } = true;
    [Parameter] public bool Loading { get; set; }
    [Parameter] public Color LoadingColor { get; set; } = Color.Primary;
    [Parameter] public bool Hover { get; set; } = true;
    [Parameter] public bool Dense { get; set; }

    private IEnumerable<SelectPropertyPath> SelectedProperties => Properties.Where(p => p.IsSelected);
}