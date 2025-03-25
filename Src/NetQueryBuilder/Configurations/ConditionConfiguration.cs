namespace NetQueryBuilder.Configurations;

public record ConditionConfiguration(IEnumerable<string> Fields, IEnumerable<string> IgnoreFields, int Depth, IEnumerable<Type> ExcludedRelationships, IPropertyStringifier? PropertyStringifier);