namespace NetQueryBuilder;

public interface IPropertyStringifier
{
    string GetName(string propertyName);
    string FormatValue(string propertyName, Type type, object? value);
}