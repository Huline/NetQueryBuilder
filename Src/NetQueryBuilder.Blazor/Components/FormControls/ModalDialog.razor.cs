using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class ModalDialog
{
    [Parameter] public bool IsVisible { get; set; }
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    [Parameter] public RenderFragment? TitleContent { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public RenderFragment? ActionContent { get; set; }
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    [Parameter] public bool CloseOnBackdropClick { get; set; } = true;

    private async Task Close()
    {
        await SetVisibility(false);
    }

    private async Task OnBackdropClick()
    {
        if (CloseOnBackdropClick)
        {
            await Close();
        }
    }

    private async Task SetVisibility(bool visible)
    {
        IsVisible = visible;
        await IsVisibleChanged.InvokeAsync(visible);
    }
}