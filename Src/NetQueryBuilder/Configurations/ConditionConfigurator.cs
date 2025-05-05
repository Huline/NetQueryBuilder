using System;

namespace NetQueryBuilder.Configurations
{
    public class ConditionConfigurator : IConditionConfigurator
    {
        private readonly ConditionConfiguration _configuration = new ConditionConfiguration(Array.Empty<string>(), Array.Empty<string>(), -1, Array.Empty<Type>(), null);

        public IConditionConfigurator LimitToFields(params string[] fields)
        {
            _configuration.Fields = fields ?? Array.Empty<string>();
            return this;
        }

        public IConditionConfigurator RemoveFields(params string[] fields)
        {
            _configuration.IgnoreFields = fields ?? Array.Empty<string>();
            return this;
        }

        public IConditionConfigurator UseStringifier(IPropertyStringifier propertyStringifier)
        {
            _configuration.PropertyStringifier = propertyStringifier;
            return this;
        }

        public IConditionConfigurator LimitDepth(int depth)
        {
            _configuration.Depth = depth;
            return this;
        }

        public IConditionConfigurator ExcludeRelationships(params Type[] relationships)
        {
            _configuration.ExcludedRelationships = relationships ?? Array.Empty<Type>();
            return this;
        }

        public ConditionConfiguration Build()
        {
            return _configuration;
        }
    }
}