using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryResultTable<TEntity> : ComponentBase
{
    private IQueryable<TEntity>? _items;
    private string _searchText = string.Empty;
    [Parameter] public QueryResult<TEntity>? Results { get; set; }
    [Parameter] [EditorRequired] public IReadOnlyCollection<SelectPropertyPath> Properties { get; set; } = new List<SelectPropertyPath>();
    [Parameter] public int TotalItems { get; set; }

    [Parameter] public bool Bordered { get; set; } = true;
    [Parameter] public bool Striped { get; set; } = true;
    [Parameter] public bool Loading { get; set; }
    [Parameter] public bool Hover { get; set; } = true;
    [Parameter] public bool Dense { get; set; }


    private IEnumerable<SelectPropertyPath> SelectedProperties => Properties.Where(p => p.IsSelected);

    private IEnumerable<TEntity> FilteredItems
    {
        get
        {
            if (Results == null || string.IsNullOrWhiteSpace(_searchText))
                return Results?.Items ?? Array.Empty<TEntity>();

            return Results.Items.Where(item =>
                SelectedProperties.Any(prop =>
                    prop.Property.DisplayValue(item)?.ToString()?.Contains(_searchText, StringComparison.OrdinalIgnoreCase) ?? false));
        }
    }

    public void Dispose()
    {
    }


    private void OnSearchChanged(ChangeEventArgs e)
    {
        _searchText = e.Value?.ToString() ?? string.Empty;
        StateHasChanged();
    }

    private Task OnPageSizeChanged(int pageSize)
    {
        if (Results != null)
        {
        }

        return Task.CompletedTask;
    }
}