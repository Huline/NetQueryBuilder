using System.Collections.ObjectModel;
using System.Windows.Input;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
/// ViewModel for the QueryBuilderContainer control.
/// </summary>
public class QueryBuilderContainerViewModel : ViewModelBase
{
    private readonly IQueryConfigurator _configurator;
    private ObservableCollection<Type> _availableEntities;
    private Type? _selectedEntityType;
    private IQuery? _currentQuery;
    private int _queryCounter = 0;

    public QueryBuilderContainerViewModel(IQueryConfigurator configurator)
    {
        _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        _availableEntities = new ObservableCollection<Type>(_configurator.GetEntities());

        NewQueryCommand = new RelayCommand(_ => NewQuery());

        // Initialize with first entity
        if (_availableEntities.Count > 0)
        {
            SelectedEntityType = _availableEntities[0];
        }
    }

    /// <summary>
    /// Gets the collection of available entity types.
    /// </summary>
    public ObservableCollection<Type> AvailableEntities
    {
        get => _availableEntities;
        set => SetProperty(ref _availableEntities, value);
    }

    /// <summary>
    /// Gets or sets the selected entity type.
    /// </summary>
    public Type? SelectedEntityType
    {
        get => _selectedEntityType;
        set
        {
            if (SetProperty(ref _selectedEntityType, value) && value != null)
            {
                CreateQueryForEntity(value);
            }
        }
    }

    /// <summary>
    /// Gets the current query.
    /// </summary>
    public IQuery? CurrentQuery
    {
        get => _currentQuery;
        private set => SetProperty(ref _currentQuery, value);
    }

    /// <summary>
    /// Gets or sets a counter to force UI refresh when creating a new query.
    /// </summary>
    public int QueryCounter
    {
        get => _queryCounter;
        set => SetProperty(ref _queryCounter, value);
    }

    /// <summary>
    /// Gets the command to create a new query.
    /// </summary>
    public ICommand NewQueryCommand { get; }

    private void NewQuery()
    {
        if (SelectedEntityType != null)
        {
            CreateQueryForEntity(SelectedEntityType);
            QueryCounter++;
        }
    }

    private void CreateQueryForEntity(Type entityType)
    {
        CurrentQuery = _configurator.BuildFor(entityType);
    }
}
