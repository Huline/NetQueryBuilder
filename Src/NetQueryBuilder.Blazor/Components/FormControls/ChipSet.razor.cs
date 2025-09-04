using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class ChipSet<TItem>
{
    [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
    [Parameter] public Func<TItem, string>? ItemToString { get; set; }
    [Parameter] public Action<TItem>? OnClose { get; set; }
}