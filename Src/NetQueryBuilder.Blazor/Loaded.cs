namespace NetQueryBuilder.Blazor;

public class Loaded : IQueryBuilderState
{
    public IQueryBuilderState DisplayBuilder(Action onDisplay)
    {
        onDisplay();
        return this;
    }
}