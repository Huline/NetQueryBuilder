namespace NetQueryBuilder.Configurations;

public interface ISelectConfigurator
{
    ISelectConfigurator LimitToFields(params string[] fields);
    ISelectConfigurator RemoveFields(params string[] fields);
    ISelectConfigurator UseStringifier(IPropertyStringifier propertyStringifier);
    ISelectConfigurator LimitDepth(int depth);
    ISelectConfigurator ExcludeRelationships(params Type[] relationships);
}