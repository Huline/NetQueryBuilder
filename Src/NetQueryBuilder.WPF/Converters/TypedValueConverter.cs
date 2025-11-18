using System.Globalization;
using System.Windows.Data;

namespace NetQueryBuilder.WPF.Converters;

/// <summary>
/// Converts between string and typed values for input controls.
/// </summary>
public class TypedValueConverter : IValueConverter
{
    public Type? TargetType { get; set; }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Convert from typed value to string for display
        return value?.ToString() ?? string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Convert from string back to typed value
        if (value is not string stringValue || string.IsNullOrWhiteSpace(stringValue))
        {
            return GetDefaultValue(TargetType ?? targetType);
        }

        var typeToConvert = TargetType ?? targetType;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;

        try
        {
            if (underlyingType == typeof(int))
                return int.Parse(stringValue, culture);

            if (underlyingType == typeof(long))
                return long.Parse(stringValue, culture);

            if (underlyingType == typeof(short))
                return short.Parse(stringValue, culture);

            if (underlyingType == typeof(byte))
                return byte.Parse(stringValue, culture);

            if (underlyingType == typeof(double))
                return double.Parse(stringValue, culture);

            if (underlyingType == typeof(float))
                return float.Parse(stringValue, culture);

            if (underlyingType == typeof(decimal))
                return decimal.Parse(stringValue, culture);

            if (underlyingType == typeof(bool))
                return bool.Parse(stringValue);

            if (underlyingType == typeof(DateTime))
                return DateTime.Parse(stringValue, culture);

            if (underlyingType == typeof(Guid))
                return Guid.Parse(stringValue);

            // Default: return as string
            return stringValue;
        }
        catch
        {
            // If parsing fails, return default value for the type
            return GetDefaultValue(typeToConvert);
        }
    }

    private static object? GetDefaultValue(Type type)
    {
        if (type == null)
            return null;

        if (type.IsValueType)
            return Activator.CreateInstance(type);

        return null;
    }
}
