using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Components.Conditions.Relations;

public partial class RelationalValue
{
    [CascadingParameter] public IQuery Query { get; set; } = null!;
    [Parameter] public SimpleCondition Condition { get; set; } = null!;

    private int GetIntValue()
    {
        return Condition.Value != null ? (int)Condition.Value : 0;
    }

    private bool GetBoolValue()
    {
        return Condition.Value != null && (bool)Condition.Value;
    }

    private DateTime? GetDateTimeValue()
    {
        return Condition.Value as DateTime?;
    }

    private void UpdateValue(DateTime? dateTime)
    {
        if (dateTime.HasValue)
        {
            UpdateValue(dateTime.Value);
        }
    }

    private void UpdateValue<T>(T value)
    {
        Condition.Value = value;
    }
}