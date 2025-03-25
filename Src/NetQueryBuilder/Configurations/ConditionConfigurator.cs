namespace NetQueryBuilder.Configurations;

public class ConditionConfigurator : IConditionConfigurator
{
    private ConditionConfiguration _configuration = new(Array.Empty<string>(), Array.Empty<string>(), -1, Array.Empty<Type>(), null);

    public IConditionConfigurator LimitToFields(params string[] fields)
    {
        _configuration = _configuration with { Fields = fields };
        return this;
    }

    public IConditionConfigurator RemoveFields(params string[] fields)
    {
        _configuration = _configuration with { IgnoreFields = fields };
        return this;
    }

    public IConditionConfigurator UseStringifier(IPropertyStringifier propertyStringifier)
    {
        _configuration = _configuration with { PropertyStringifier = propertyStringifier };
        return this;
    }

    public IConditionConfigurator LimitDepth(int depth)
    {
        _configuration = _configuration with { Depth = depth };
        return this;
    }

    public IConditionConfigurator ExcludeRelationships(params Type[] relationships)
    {
        _configuration = _configuration with { ExcludedRelationships = relationships };
        return this;
    }

    public ConditionConfiguration Build()
    {
        return _configuration;
    }
}