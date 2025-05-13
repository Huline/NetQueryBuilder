using NetQueryBuilder.Properties;

namespace NetQueryBuilder.EntityFramework.Tests.Data;

public class TestStringifier : IPropertyStringifier
{
    public string GetName(string propertyName)
    {
        return propertyName.ToUpper();
    }

    public string FormatValue(string propertyName, Type type, object? value)
    {
        return value?.ToString()?.ToUpper() ?? string.Empty;
    }
}