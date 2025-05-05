using System;

namespace NetQueryBuilder.Configurations
{
    public class SelectConfigurator : ISelectConfigurator
    {
        private readonly SelectConfiguration _configuration = new SelectConfiguration(Array.Empty<string>(), Array.Empty<string>(), -1, Array.Empty<Type>(), null);

        public ISelectConfigurator LimitToFields(params string[] fields)
        {
            _configuration.Fields = fields ?? Array.Empty<string>();
            return this;
        }

        public ISelectConfigurator RemoveFields(params string[] fields)
        {
            _configuration.IgnoreFields = fields ?? Array.Empty<string>();
            return this;
        }

        public ISelectConfigurator UseStringifier(IPropertyStringifier propertyStringifier)
        {
            _configuration.PropertyStringifier = propertyStringifier;
            return this;
        }

        public ISelectConfigurator LimitDepth(int depth)
        {
            _configuration.Depth = depth;
            return this;
        }

        public ISelectConfigurator ExcludeRelationships(params Type[] relationships)
        {
            _configuration.ExcludedRelationships = relationships ?? Array.Empty<Type>();
            return this;
        }

        public SelectConfiguration Build()
        {
            return _configuration;
        }
    }
}