namespace NetQueryBuilder.Blazor;

public interface IQueryBuilderState
{
    IQueryBuilderState DisplayBuilder(Action onDisplay);
}