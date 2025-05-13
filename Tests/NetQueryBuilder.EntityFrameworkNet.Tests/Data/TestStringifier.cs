using System;
using NetQueryBuilder.Properties;

namespace NetQueryBuilder.EntityFrameworkNet.Tests.Data
{
    public class TestStringifier : IPropertyStringifier
    {
        public string GetName(string propertyName)
        {
            return propertyName.Replace(".", "_").ToUpper();
        }

        public string FormatValue(string propertyName, Type type, object value)
        {
            return value?.ToString()?.ToUpper() ?? string.Empty;
        }
    }
}