namespace NetQueryBuilder.Configurations;

public class SelectConfigurator : ISelectConfigurator
{
    private SelectConfiguration _configuration = new(Array.Empty<string>(), Array.Empty<string>(), -1, Array.Empty<Type>(), null);

    public ISelectConfigurator LimitToFields(params string[] fields)
    {
        _configuration = _configuration with { Fields = fields };
        return this;
    }

    public ISelectConfigurator RemoveFields(params string[] fields)
    {
        _configuration = _configuration with { IgnoreFields = fields };
        return this;
    }

    public ISelectConfigurator UseStringifier(IPropertyStringifier propertyStringifier)
    {
        _configuration = _configuration with { PropertyStringifier = propertyStringifier };
        return this;
    }

    public ISelectConfigurator LimitDepth(int depth)
    {
        _configuration = _configuration with { Depth = depth };
        return this;
    }

    public ISelectConfigurator ExcludeRelationships(params Type[] relationships)
    {
        _configuration = _configuration with { ExcludedRelationships = relationships };
        return this;
    }

    public SelectConfiguration Build()
    {
        return _configuration;
    }
}