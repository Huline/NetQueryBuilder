using System.Collections.ObjectModel;
using System.Windows.Input;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;
using NetQueryBuilder.WPF.Commands;

namespace NetQueryBuilder.WPF.ViewModels;

/// <summary>
/// ViewModel for a simple condition (Property Operator Value).
/// </summary>
public class SimpleConditionViewModel : ViewModelBase
{
    private readonly IQuery _query;
    private readonly SimpleCondition _condition;
    private ObservableCollection<PropertyPath> _availableProperties;
    private ObservableCollection<ExpressionOperator> _availableOperators;
    private PropertyPath? _selectedProperty;
    private ExpressionOperator? _selectedOperator;
    private object? _value;
    private Type? _propertyType;

    public SimpleConditionViewModel(IQuery query, SimpleCondition condition)
    {
        _query = query ?? throw new ArgumentNullException(nameof(query));
        _condition = condition ?? throw new ArgumentNullException(nameof(condition));

        _availableProperties = new ObservableCollection<PropertyPath>(_query.ConditionPropertyPaths);
        _selectedProperty = _condition.PropertyPath;
        _selectedOperator = _condition.Operator;
        _value = _condition.Value;
        _propertyType = _condition.PropertyPath.PropertyType;
        _availableOperators = new ObservableCollection<ExpressionOperator>(_condition.AvailableOperatorsForCurrentProperty());

        DeleteCommand = new RelayCommand(_ => OnDelete());
    }

    /// <summary>
    /// Gets the collection of available properties to choose from.
    /// </summary>
    public ObservableCollection<PropertyPath> AvailableProperties
    {
        get => _availableProperties;
        set => SetProperty(ref _availableProperties, value);
    }

    /// <summary>
    /// Gets the collection of available operators for the selected property.
    /// </summary>
    public ObservableCollection<ExpressionOperator> AvailableOperators
    {
        get => _availableOperators;
        set => SetProperty(ref _availableOperators, value);
    }

    /// <summary>
    /// Gets or sets the selected property.
    /// </summary>
    public PropertyPath? SelectedProperty
    {
        get => _selectedProperty;
        set
        {
            if (SetProperty(ref _selectedProperty, value) && value != null)
            {
                _condition.PropertyPath = value;
                PropertyType = value.PropertyType;
                AvailableOperators = new ObservableCollection<ExpressionOperator>(_condition.AvailableOperatorsForCurrentProperty());
                SelectedOperator = AvailableOperators.FirstOrDefault();
            }
        }
    }

    /// <summary>
    /// Gets or sets the selected operator.
    /// </summary>
    public ExpressionOperator? SelectedOperator
    {
        get => _selectedOperator;
        set
        {
            if (SetProperty(ref _selectedOperator, value) && value != null)
            {
                _condition.Operator = value;
                Value = _condition.Value; // Refresh value from condition (may have changed due to operator change)
            }
        }
    }

    /// <summary>
    /// Gets or sets the condition value.
    /// </summary>
    public object? Value
    {
        get => _value;
        set
        {
            if (SetProperty(ref _value, value))
            {
                _condition.Value = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the property type (for template selection).
    /// </summary>
    public Type? PropertyType
    {
        get => _propertyType;
        set => SetProperty(ref _propertyType, value);
    }

    /// <summary>
    /// Gets the command to delete this condition.
    /// </summary>
    public ICommand DeleteCommand { get; }

    /// <summary>
    /// Occurs when the delete button is clicked.
    /// </summary>
    public event EventHandler? DeleteRequested;

    private void OnDelete()
    {
        DeleteRequested?.Invoke(this, EventArgs.Empty);
    }
}
