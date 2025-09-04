using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Blazor.Components.Conditions;

public partial class ConditionComponent
{
    [Parameter] public ICondition Condition { get; set; } = null!;
}