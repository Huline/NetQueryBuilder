public class QueryResultPageState
{
    public int PageSize { get; set; } = 10;
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages => (TotalItems + PageSize - 1) / PageSize;

    public event EventHandler? StateChanged;

    public void OnPageChanged(int page)
    {
        CurrentPage = page;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }

    public void OnPageSizeChanged(int pageSize)
    {
        PageSize = pageSize;
        CurrentPage = 0;
        StateChanged?.Invoke(this, EventArgs.Empty);
    }
}