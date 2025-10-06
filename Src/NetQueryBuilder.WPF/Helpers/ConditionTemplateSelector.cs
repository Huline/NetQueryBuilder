using System.Windows;
using System.Windows.Controls;
using NetQueryBuilder.Conditions;

namespace NetQueryBuilder.WPF.Helpers;

/// <summary>
/// Selects the appropriate template for a condition (Block or Simple).
/// </summary>
public class ConditionTemplateSelector : DataTemplateSelector
{
    public DataTemplate? BlockConditionTemplate { get; set; }
    public DataTemplate? SimpleConditionTemplate { get; set; }

    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        return item switch
        {
            BlockCondition => BlockConditionTemplate,
            SimpleCondition => SimpleConditionTemplate,
            _ => base.SelectTemplate(item, container)
        };
    }
}
