using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class SwitchControl
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public bool Value { get; set; }
    [Parameter] public EventCallback<bool> ValueChanged { get; set; }
    [Parameter] public bool Disabled { get; set; } = false;

    private async Task OnValueChanged(ChangeEventArgs e)
    {
        if (e.Value != null && bool.TryParse(e.Value.ToString(), out bool result))
        {
            Value = result;
            await ValueChanged.InvokeAsync(Value);
        }
    }
}