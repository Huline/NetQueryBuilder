using System.Collections.ObjectModel;
using System.Windows.Input;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
/// ViewModel for the QueryResultGrid control.
/// </summary>
public class QueryResultViewModel : ViewModelBase
{
    private QueryResult<dynamic>? _results;
    private ObservableCollection<SelectPropertyPath>? _displayProperties;
    private int _currentPage = 1;
    private int _totalPages = 0;
    private int _totalItems = 0;

    public QueryResultViewModel()
    {
        GoToFirstPageCommand = new AsyncRelayCommand(async _ => await GoToPageAsync(1), _ => CanGoToPreviousPage);
        GoToPreviousPageCommand = new AsyncRelayCommand(async _ => await GoToPageAsync(CurrentPage - 1), _ => CanGoToPreviousPage);
        GoToNextPageCommand = new AsyncRelayCommand(async _ => await GoToPageAsync(CurrentPage + 1), _ => CanGoToNextPage);
        GoToLastPageCommand = new AsyncRelayCommand(async _ => await GoToPageAsync(TotalPages), _ => CanGoToNextPage);
    }

    /// <summary>
    /// Gets or sets the query results.
    /// </summary>
    public QueryResult<dynamic>? Results
    {
        get => _results;
        set
        {
            if (SetProperty(ref _results, value))
            {
                if (_results != null)
                {
                    CurrentPage = _results.CurrentPage + 1; // Convert from 0-based to 1-based
                    TotalPages = _results.TotalPage;
                    TotalItems = _results.TotalItems;
                }
                OnPropertyChanged(nameof(HasResults));
                OnPropertyChanged(nameof(Items));
            }
        }
    }

    /// <summary>
    /// Gets or sets the properties to display as columns.
    /// </summary>
    public ObservableCollection<SelectPropertyPath>? DisplayProperties
    {
        get => _displayProperties;
        set => SetProperty(ref _displayProperties, value);
    }

    /// <summary>
    /// Gets the current page number (1-based).
    /// </summary>
    public int CurrentPage
    {
        get => _currentPage;
        set
        {
            if (SetProperty(ref _currentPage, value))
            {
                OnPropertyChanged(nameof(CanGoToPreviousPage));
                OnPropertyChanged(nameof(CanGoToNextPage));
            }
        }
    }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    public int TotalPages
    {
        get => _totalPages;
        set => SetProperty(ref _totalPages, value);
    }

    /// <summary>
    /// Gets the total number of items.
    /// </summary>
    public int TotalItems
    {
        get => _totalItems;
        set => SetProperty(ref _totalItems, value);
    }

    /// <summary>
    /// Gets whether there are results to display.
    /// </summary>
    public bool HasResults => Results?.Items?.Count > 0;

    /// <summary>
    /// Gets the items to display.
    /// </summary>
    public IReadOnlyCollection<dynamic>? Items => Results?.Items;

    /// <summary>
    /// Gets whether navigation to previous page is possible.
    /// </summary>
    public bool CanGoToPreviousPage => CurrentPage > 1;

    /// <summary>
    /// Gets whether navigation to next page is possible.
    /// </summary>
    public bool CanGoToNextPage => CurrentPage < TotalPages;

    /// <summary>
    /// Command to go to the first page.
    /// </summary>
    public ICommand GoToFirstPageCommand { get; }

    /// <summary>
    /// Command to go to the previous page.
    /// </summary>
    public ICommand GoToPreviousPageCommand { get; }

    /// <summary>
    /// Command to go to the next page.
    /// </summary>
    public ICommand GoToNextPageCommand { get; }

    /// <summary>
    /// Command to go to the last page.
    /// </summary>
    public ICommand GoToLastPageCommand { get; }

    private async Task GoToPageAsync(int pageNumber)
    {
        if (Results == null || pageNumber < 1 || pageNumber > TotalPages)
            return;

        // Convert from 1-based to 0-based for the API
        Results = await Results.GoToPage(pageNumber - 1);
    }
}
