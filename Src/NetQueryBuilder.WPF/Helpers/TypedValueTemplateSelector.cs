using System.Windows;
using System.Windows.Controls;

namespace NetQueryBuilder.WPF.Helpers;

/// <summary>
/// Selects a DataTemplate based on the property type for value input.
/// </summary>
public class TypedValueTemplateSelector : DataTemplateSelector
{
    public DataTemplate? StringTemplate { get; set; }
    public DataTemplate? IntegerTemplate { get; set; }
    public DataTemplate? DecimalTemplate { get; set; }
    public DataTemplate? BooleanTemplate { get; set; }
    public DataTemplate? DateTimeTemplate { get; set; }
    public DataTemplate? EnumTemplate { get; set; }
    public DataTemplate? ListTemplate { get; set; }
    public DataTemplate? DefaultTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (item is not Type type)
            return DefaultTemplate;

        if (type == typeof(string))
            return StringTemplate;
        if (type == typeof(int) || type == typeof(long) || type == typeof(short) || type == typeof(byte))
            return IntegerTemplate;
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float))
            return DecimalTemplate;
        if (type == typeof(bool))
            return BooleanTemplate;
        if (type == typeof(DateTime) || type == typeof(DateTimeOffset) || type == typeof(DateOnly))
            return DateTimeTemplate;
        if (type.IsEnum)
            return EnumTemplate;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            return ListTemplate;

        return DefaultTemplate;
    }
}
