using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NetQueryBuilder.WPF.Converters;

/// <summary>
/// Converts null values to Visibility (null = Collapsed, not null = Visible).
/// </summary>
public class NullToVisibilityConverter : IValueConverter
{
    /// <summary>
    /// Gets or sets whether to invert the conversion (null = Visible, not null = Collapsed).
    /// </summary>
    public bool Invert { get; set; }

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool isNull = value == null;

        if (Invert)
            isNull = !isNull;

        return isNull ? Visibility.Collapsed : Visibility.Visible;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
