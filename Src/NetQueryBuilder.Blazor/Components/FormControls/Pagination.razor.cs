using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class Pagination
{
    [Parameter] public int[] PageSizeOptions { get; set; } = { 10, 25, 50, 100 };
    [Parameter] public int PageSize { get; set; } = 10;
    [Parameter] public int CurrentPage { get; set; } = 1;
    [Parameter] public int TotalItems { get; set; } = 0;
    [Parameter] public EventCallback<int> PageChanged { get; set; }
    [Parameter] public EventCallback<int> PageSizeChanged { get; set; }
    private int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
    private int StartItem => Math.Min((CurrentPage - 1) * PageSize + 1, TotalItems);
    private int EndItem => Math.Min(CurrentPage * PageSize, TotalItems);

    private async Task OnPageChanged(int page)
    {
        if (page < 1 || page > TotalPages) return;

        await PageChanged.InvokeAsync(page);
    }

    private async Task OnPageSizeChanged(ChangeEventArgs e)
    {
        if (e.Value != null && int.TryParse(e.Value.ToString(), out int size))
        {
            await PageSizeChanged.InvokeAsync(size);
        }
    }
}