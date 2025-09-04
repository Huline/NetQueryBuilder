using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class NumericField<TValue>
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string HelperText { get; set; } = string.Empty;
    [Parameter] public TValue Value { get; set; }
    [Parameter] public EventCallback<TValue> ValueChanged { get; set; }
    [Parameter] public string AdornmentIcon { get; set; } = string.Empty;
    [Parameter] public string Step { get; set; } = "1";
    [Parameter] public string Min { get; set; } = string.Empty;
    [Parameter] public string Max { get; set; } = string.Empty;
    [Parameter] public bool Disabled { get; set; } = false;

    private async Task OnValueChanged(ChangeEventArgs e)
    {
        if (e.Value != null && TryParse(e.Value.ToString(), out TValue result))
        {
            Value = result;
            await ValueChanged.InvokeAsync(Value);
        }
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        // On pourrait ajouter des validations spécifiques ici si nécessaire
    }

    private bool TryParse(string value, out TValue result)
    {
        try
        {
            if (typeof(TValue) == typeof(int))
            {
                if (int.TryParse(value, out var intResult))
                {
                    result = (TValue)(object)intResult;
                    return true;
                }
            }
            else if (typeof(TValue) == typeof(decimal))
            {
                if (decimal.TryParse(value, out var decimalResult))
                {
                    result = (TValue)(object)decimalResult;
                    return true;
                }
            }
            else if (typeof(TValue) == typeof(double))
            {
                if (double.TryParse(value, out var doubleResult))
                {
                    result = (TValue)(object)doubleResult;
                    return true;
                }
            }
            // Ajoutez d'autres types numériques au besoin

            result = default;
            return false;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}