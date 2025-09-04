using Microsoft.AspNetCore.Components;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.Blazor.Components.Conditions;

public partial class SimpleConditionComponent
{
    [Parameter] public SimpleCondition Condition { get; set; } = null!;
}