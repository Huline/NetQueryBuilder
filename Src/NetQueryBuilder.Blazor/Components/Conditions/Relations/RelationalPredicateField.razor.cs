using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components.Conditions.Relations;

public partial class RelationalPredicateField
{
    [CascadingParameter] public IQuery Query { get; set; } = null!;
    [Parameter] public SimpleCondition Condition { get; set; } = null!;
    private PropertyPath? _selectedPropertyName;
    private IEnumerable<PropertyPath> _properties = new List<PropertyPath>();

    protected override async Task OnInitializedAsync()
    {
        _properties = Query.ConditionPropertyPaths;
        _selectedPropertyName = Condition.PropertyPath;
        await base.OnInitializedAsync();
    }

    private void OnFieldSelected(PropertyPath? propertyName)
    {
        _selectedPropertyName = propertyName;
        Condition.PropertyPath = propertyName;
    }
}