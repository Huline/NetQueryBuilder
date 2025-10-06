using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace NetQueryBuilder.WPF.Converters;

/// <summary>
/// Converts boolean to background color (true = highlighted, false = transparent).
/// </summary>
public class BooleanToBackgroundConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool boolValue = value is bool b && b;
        return boolValue ? new SolidColorBrush(Color.FromRgb(232, 244, 253)) : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
