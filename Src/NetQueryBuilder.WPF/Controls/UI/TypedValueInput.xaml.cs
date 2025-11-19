using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NetQueryBuilder.WPF.Controls.UI;

/// <summary>
/// A type-aware value input control that shows different UI based on the property type.
/// </summary>
public partial class TypedValueInput : UserControl
{
    private static readonly Regex NumericRegex = new Regex("[^0-9-]+");
    private static readonly Regex DecimalRegex = new Regex("[^0-9.-]+");

    public static readonly DependencyProperty PropertyTypeProperty =
        DependencyProperty.Register(
            nameof(PropertyType),
            typeof(Type),
            typeof(TypedValueInput),
            new PropertyMetadata(typeof(string)));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(
            nameof(Value),
            typeof(object),
            typeof(TypedValueInput),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public TypedValueInput()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the property type to determine which input control to show.
    /// </summary>
    public Type PropertyType
    {
        get => (Type)GetValue(PropertyTypeProperty);
        set => SetValue(PropertyTypeProperty, value);
    }

    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public object? Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    private void NumericTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = NumericRegex.IsMatch(e.Text);
    }

    private void DecimalTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = DecimalRegex.IsMatch(e.Text);
    }
}
