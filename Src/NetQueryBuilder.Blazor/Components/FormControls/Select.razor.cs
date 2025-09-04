using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class Select<TItem>
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
    [Parameter] public TItem? Value { get; set; }
    [Parameter] public EventCallback<TItem?> ValueChanged { get; set; }
    [Parameter] public Func<TItem, string>? ToStringFunc { get; set; }

    private async Task OnChange(ChangeEventArgs e)
    {
        if (ToStringFunc == null) return;

        var stringValue = e.Value?.ToString();
        var selected = Items.FirstOrDefault(x => ToStringFunc(x) == stringValue);
        Value = selected;
        await ValueChanged.InvokeAsync(Value);
    }

    protected override void OnParametersSet()
    {
        if (ToStringFunc == null && Items.Any())
        {
            ToStringFunc = x => x?.ToString() ?? string.Empty;
        }
    }
}