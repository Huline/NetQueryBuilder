using Microsoft.AspNetCore.Components;

namespace NetQueryBuilder.Blazor.Components;

public partial class QueryBuilderContainer
{
    private Type? _selectedEntityType;
    private int _queryBuilderKey; // Unique key to force re-render
    private IReadOnlyList<Type> _entities = Array.Empty<Type>();

    private RenderFragment CreateQueryBuilder()
    {
        if (_selectedEntityType is null)
        {
            return builder =>
            {
                builder.OpenElement(0, "div");
                builder.AddAttribute(1, "style", "text-align: center; margin-top: 20px;");
                builder.AddContent(2, "Please select an entity.");
                builder.CloseElement();
            };
        }

        return builder =>
        {
            builder.OpenComponent(0, typeof(QueryBuilder<>).MakeGenericType(_selectedEntityType));
            builder.AddAttribute(1, "Expression", Expression);
            builder.CloseComponent();
        };
    }

    private string Expression { get; set; } = string.Empty;

    protected override void OnInitialized()
    {
        _entities = QueryConfigurator.GetEntities().ToList();
        NewQuery();
        base.OnInitializedAsync();
    }

    public void OnEntitySelect(Type? value)
    {
        Expression = string.Empty;
        SelectEntity(value);
    }

    private void SelectEntity(Type? entityType)
    {
        _selectedEntityType = entityType;
        StateHasChanged();
    }

    public void NewQuery()
    {
        SelectEntity(_entities.First());
        Expression = string.Empty;
        _queryBuilderKey++;
    }
}