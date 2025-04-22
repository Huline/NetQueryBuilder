using System;
using System.Collections.Generic;

namespace NetQueryBuilder.Configurations
{
    public class ConditionConfiguration
    {
        public ConditionConfiguration(IEnumerable<string> fields, IEnumerable<string> ignoreFields, int depth, IEnumerable<Type> excludedRelationships, IPropertyStringifier? propertyStringifier)
        {
            Fields = fields;
            IgnoreFields = ignoreFields;
            Depth = depth;
            ExcludedRelationships = excludedRelationships;
            PropertyStringifier = propertyStringifier;
        }

        public IEnumerable<string> Fields { get; set; }
        public IEnumerable<string> IgnoreFields { get; set; }
        public int Depth { get; set; }
        public IEnumerable<Type> ExcludedRelationships { get; set; }
        public IPropertyStringifier? PropertyStringifier { get; set; }
    }
}