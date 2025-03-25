namespace NetQueryBuilder.Configurations;

public record SelectConfiguration(IEnumerable<string> Fields, IEnumerable<string> IgnoreFields, int Depth, IEnumerable<Type> ExcludedRelationships, IPropertyStringifier? PropertyStringifier);