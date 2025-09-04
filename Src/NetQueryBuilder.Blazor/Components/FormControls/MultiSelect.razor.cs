using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace NetQueryBuilder.Blazor.Components.FormControls;

public partial class MultiSelect<TItem>
{
    [Parameter] public string Label { get; set; } = string.Empty;
    [Parameter] public IEnumerable<TItem> Items { get; set; } = Array.Empty<TItem>();
    [Parameter] public IEnumerable<TItem> SelectedValues { get; set; } = Array.Empty<TItem>();
    [Parameter] public EventCallback<IEnumerable<TItem>> SelectedValuesChanged { get; set; }
    [Parameter] public Func<TItem?, string>? ToStringFunc { get; set; }
    [Parameter] public string AdornmentIcon { get; set; } = string.Empty;
    [Parameter] public bool SelectAll { get; set; }
    private List<TItem> _selectedItems = new();

    protected override void OnParametersSet()
    {
        _selectedItems = SelectedValues.ToList();
    }

    private bool IsSelected(TItem item)
    {
        return _selectedItems.Contains(item);
    }

    private bool AreAllSelected()
    {
        return Items.All(i => _selectedItems.Contains(i));
    }

    private async Task ToggleItem(TItem item, bool isChecked)
    {
        if (isChecked)
        {
            if (!_selectedItems.Contains(item))
            {
                _selectedItems.Add(item);
            }
        }
        else
        {
            _selectedItems.Remove(item);
        }

        await SelectedValuesChanged.InvokeAsync(_selectedItems);
    }

    private async Task ToggleSelectAll(ChangeEventArgs e)
    {
        if (e.Value != null && (bool)e.Value)
        {
            _selectedItems = Items.ToList();
        }
        else
        {
            _selectedItems.Clear();
        }

        await SelectedValuesChanged.InvokeAsync(_selectedItems);
    }

    protected override Task OnInitializedAsync()
    {
        return Task.CompletedTask;
    }
}