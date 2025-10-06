using System.Globalization;
using System.Windows.Data;

namespace NetQueryBuilder.WPF.Converters;

/// <summary>
/// Converts a Type to a control type identifier for template selection.
/// </summary>
public class TypeToControlConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not Type type)
            return "Unknown";

        if (type == typeof(string))
            return "String";
        if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
            return "Integer";
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            return "Decimal";
        if (type == typeof(bool))
            return "Boolean";
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly))
            return "DateTime";
        if (type.IsEnum)
            return "Enum";
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return "List";

        return "Unknown";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
