using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NetQueryBuilder.WPF.Converters;

/// <summary>
/// Converts boolean values to Visibility values.
/// </summary>
public class BooleanToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets whether to invert the conversion (true = Collapsed, false = Visible).
    /// </summary>
    public bool Invert { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;

        if (Invert)
            boolValue = !boolValue;

        return boolValue ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool result = value is Visibility visibility && visibility == Visibility.Visible;

        if (Invert)
            result = !result;

        return result;
    }
}
