using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class TextField
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string HelperText { get; set; } = string.Empty;
    [Parameter] public string Value { get; set; } = string.Empty;
    [Parameter] public EventCallback<string> ValueChanged { get; set; }
    [Parameter] public string AdornmentIcon { get; set; } = string.Empty;
    [Parameter] public string Placeholder { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; } = false;

    private async Task OnValueChanged(ChangeEventArgs e)
    {
        if (e.Value != null)
        {
            Value = e.Value.ToString() ?? string.Empty;
            await ValueChanged.InvokeAsync(Value);
        }
    }
}