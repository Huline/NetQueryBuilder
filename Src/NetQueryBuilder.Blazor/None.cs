namespace NetQueryBuilder.Blazor;

public class None : IQueryBuilderState
{
    public IQueryBuilderState DisplayBuilder(Action onDisplay)
    {
        return this;
    }
}