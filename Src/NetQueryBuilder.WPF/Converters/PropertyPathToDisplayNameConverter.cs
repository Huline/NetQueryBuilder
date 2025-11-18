using System.Globalization;
using System.Windows.Data;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.WPF.Converters;

/// <summary>
/// Converts a PropertyPath to its display name.
/// </summary>
public class PropertyPathToDisplayNameConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is PropertyPath propertyPath)
        {
            return propertyPath.DisplayName();
        }
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
