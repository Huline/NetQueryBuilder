using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.WPF.Controls.UI;

/// <summary>
/// A ComboBox for selecting operators.
/// </summary>
public partial class OperatorSelector : UserControl
{
    public static readonly DependencyProperty OperatorsProperty =
        DependencyProperty.Register(
            nameof(Operators),
            typeof(ObservableCollection<ExpressionOperator>),
            typeof(OperatorSelector),
            new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedOperatorProperty =
        DependencyProperty.Register(
            nameof(SelectedOperator),
            typeof(ExpressionOperator),
            typeof(OperatorSelector),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public OperatorSelector()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the collection of available operators.
    /// </summary>
    public ObservableCollection<ExpressionOperator> Operators
    {
        get => (ObservableCollection<ExpressionOperator>)GetValue(OperatorsProperty);
        set => SetValue(OperatorsProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected operator.
    /// </summary>
    public ExpressionOperator? SelectedOperator
    {
        get => (ExpressionOperator?)GetValue(SelectedOperatorProperty);
        set => SetValue(SelectedOperatorProperty, value);
    }
}
