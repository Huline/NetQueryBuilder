using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class DatePicker
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public string HelperText { get; set; } = string.Empty;
    [Parameter] public DateTime? Date { get; set; }
    [Parameter] public EventCallback<DateTime?> DateChanged { get; set; }
    [Parameter] public string AdornmentIcon { get; set; } = "📅"; // Emoji par défaut pour le calendrier
    [Parameter] public bool Disabled { get; set; } = false;

    private string FormatDateForInput(DateTime? date)
    {
        return date?.ToString("yyyy-MM-dd") ?? string.Empty;
    }

    private async Task OnDateChanged(ChangeEventArgs e)
    {
        if (e.Value != null && !string.IsNullOrEmpty(e.Value.ToString()))
        {
            if (DateTime.TryParse(e.Value.ToString(), out DateTime result))
            {
                Date = result;
                await DateChanged.InvokeAsync(Date);
            }
        }
        else
        {
            Date = null;
            await DateChanged.InvokeAsync(Date);
        }
    }
}