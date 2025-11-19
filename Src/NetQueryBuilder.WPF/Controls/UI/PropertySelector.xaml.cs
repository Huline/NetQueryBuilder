using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using NqbPropertyPath = NetQueryBuilder.Properties.PropertyPath;

namespace NetQueryBuilder.WPF.Controls.UI;

/// <summary>
/// A ComboBox for selecting properties with display names.
/// </summary>
public partial class PropertySelector : UserControl
{
    public static readonly DependencyProperty PropertiesProperty =
        DependencyProperty.Register(
            nameof(Properties),
            typeof(ObservableCollection<NqbPropertyPath>),
            typeof(PropertySelector),
            new PropertyMetadata(null));

    public static readonly DependencyProperty SelectedPropertyProperty =
        DependencyProperty.Register(
            nameof(SelectedProperty),
            typeof(NqbPropertyPath),
            typeof(PropertySelector),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public PropertySelector()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Gets or sets the collection of available properties.
    /// </summary>
    public ObservableCollection<NqbPropertyPath> Properties
    {
        get => (ObservableCollection<NqbPropertyPath>)GetValue(PropertiesProperty);
        set => SetValue(PropertiesProperty, value);
    }

    /// <summary>
    /// Gets or sets the currently selected property.
    /// </summary>
    public NqbPropertyPath? SelectedProperty
    {
        get => (NqbPropertyPath?)GetValue(SelectedPropertyProperty);
        set => SetValue(SelectedPropertyProperty, value);
    }
}
