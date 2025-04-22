using System;

namespace NetQueryBuilder.Configurations
{
    public interface IConditionConfigurator
    {
        IConditionConfigurator LimitToFields(params string[]? fields);
        IConditionConfigurator RemoveFields(params string[]? fields);
        IConditionConfigurator UseStringifier(IPropertyStringifier propertyStringifier);
        IConditionConfigurator LimitDepth(int depth);
        IConditionConfigurator ExcludeRelationships(params Type[]? relationships);
    }
}