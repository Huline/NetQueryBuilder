namespace NetQueryBuilder.Blazor;

public class New : IQueryBuilderState
{
    public IQueryBuilderState DisplayBuilder(Action onDisplay)
    {
        onDisplay();
        return this;
    }
}