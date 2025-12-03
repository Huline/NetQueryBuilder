using Microsoft.AspNetCore.Razor.TagHelpers;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Conditions;
using System.Text;

namespace NetQueryBuilder.AspNetCore.TagHelpers;

/// <summary>
/// Tag helper for rendering the condition builder (WHERE clause)
/// </summary>
[HtmlTargetElement("nqb-condition-builder")]
public class ConditionBuilderTagHelper : TagHelper
{
    private readonly IQuerySessionService _sessionService;

    public ConditionBuilderTagHelper(IQuerySessionService sessionService)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
    }

    /// <summary>
    /// Session ID for retrieving state
    /// </summary>
    [HtmlAttributeName("session-id")]
    public string? SessionId { get; set; }

    /// <summary>
    /// CSS class to apply to the container
    /// </summary>
    [HtmlAttributeName("class")]
    public string? CssClass { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (string.IsNullOrEmpty(SessionId))
        {
            output.SuppressOutput();
            return;
        }

        var state = _sessionService.GetState(SessionId);

        // If no query, don't render
        if (state.Query == null)
        {
            output.SuppressOutput();
            return;
        }

        // Change output tag
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"nqb-condition-builder {CssClass}".Trim());

        // Build HTML
        var html = new StringBuilder();

        html.AppendLine("<h3 class=\"nqb-section-title\">WHERE Conditions</h3>");
        html.AppendLine("<p class=\"nqb-hint\">Build conditions to filter your query results</p>");

        // Render root block condition
        RenderBlockCondition(html, state.Query.Condition, 0, state.Query.ConditionPropertyPaths);

        output.Content.SetHtmlContent(html.ToString());
    }

    private void RenderBlockCondition(StringBuilder html, BlockCondition block, int level, IReadOnlyCollection<NetQueryBuilder.Properties.PropertyPath> availableProperties)
    {
        var indent = level * 20; // 20px per level

        html.AppendLine($"<div class=\"nqb-block-condition\" style=\"margin-left: {indent}px;\">");

        // Header with add buttons
        html.AppendLine("  <div class=\"nqb-block-header\">");
        html.AppendLine("    <button type=\"submit\" formmethod=\"post\" formaction=\"?handler=AddCondition\" class=\"nqb-button nqb-button-small nqb-button-outlined\">");
        html.AppendLine("      + Add Condition");
        html.AppendLine("    </button>");
        html.AppendLine("    <button type=\"submit\" formmethod=\"post\" formaction=\"?handler=AddGroup\" class=\"nqb-button nqb-button-small nqb-button-outlined\">");
        html.AppendLine("      + Add Group");
        html.AppendLine("    </button>");

        if (level > 0)
        {
            html.AppendLine("    <button type=\"submit\" formmethod=\"post\" formaction=\"?handler=Ungroup\" class=\"nqb-button nqb-button-small nqb-button-text\">");
            html.AppendLine("      Ungroup");
            html.AppendLine("    </button>");
        }

        html.AppendLine("  </div>");

        // Render children
        if (block.Conditions.Any())
        {
            html.AppendLine("  <div class=\"nqb-conditions-list\">");

            var conditions = block.Conditions.ToList();
            for (int i = 0; i < conditions.Count; i++)
            {
                var condition = conditions[i];

                if (condition is SimpleCondition simple)
                {
                    RenderSimpleCondition(html, simple, i, i > 0, availableProperties);
                }
                else if (condition is BlockCondition childBlock)
                {
                    // Logical operator before nested block
                    if (i > 0)
                    {
                        RenderLogicalOperatorSelect(html, i, childBlock.LogicalOperator);
                    }

                    RenderBlockCondition(html, childBlock, level + 1, availableProperties);
                }

                // Separator between conditions (except last)
                if (i < conditions.Count - 1 && condition is SimpleCondition)
                {
                    RenderLogicalOperatorSelect(html, i + 1, conditions[i + 1].LogicalOperator);
                }
            }

            html.AppendLine("  </div>");
        }
        else
        {
            html.AppendLine("  <p class=\"nqb-empty-message\">No conditions yet. Click 'Add Condition' to start building your query.</p>");
        }

        html.AppendLine("</div>");
    }

    private void RenderSimpleCondition(StringBuilder html, SimpleCondition condition, int index, bool showLogicalOperator, IReadOnlyCollection<NetQueryBuilder.Properties.PropertyPath> availableProperties)
    {
        html.AppendLine("  <div class=\"nqb-simple-condition\">");
        html.AppendLine("    <div class=\"nqb-condition-row\">");

        // Property selector - show all available properties
        html.AppendLine("      <div class=\"nqb-condition-field\">");
        html.AppendLine("        <label class=\"nqb-label-small\">Property</label>");
        html.AppendLine($"        <select name=\"Conditions[{index}].PropertyPath\" class=\"nqb-select nqb-select-small\">");
        foreach (var prop in availableProperties)
        {
            var selected = prop.PropertyFullName == condition.PropertyPath.PropertyFullName ? " selected" : "";
            html.AppendLine($"          <option value=\"{prop.PropertyFullName}\"{selected}>{prop.PropertyName}</option>");
        }
        html.AppendLine("        </select>");
        html.AppendLine("      </div>");

        // Operator selector - show all compatible operators for the current property
        html.AppendLine("      <div class=\"nqb-condition-field\">");
        html.AppendLine("        <label class=\"nqb-label-small\">Operator</label>");
        html.AppendLine($"        <select name=\"Conditions[{index}].Operator\" class=\"nqb-select nqb-select-small\">");
        var compatibleOperators = condition.PropertyPath.GetCompatibleOperators();
        foreach (var op in compatibleOperators)
        {
            var selected = op.GetType().Name == condition.Operator.GetType().Name ? " selected" : "";
            html.AppendLine($"          <option value=\"{op.GetType().Name}\"{selected}>{op}</option>");
        }
        html.AppendLine("        </select>");
        html.AppendLine("      </div>");

        // Value input
        html.AppendLine("      <div class=\"nqb-condition-field\">");
        html.AppendLine("        <label class=\"nqb-label-small\">Value</label>");
        html.AppendLine($"        <input type=\"text\" name=\"Conditions[{index}].Value\" value=\"{condition.Value}\" class=\"nqb-textfield nqb-textfield-small\" />");
        html.AppendLine("      </div>");

        // Remove button
        html.AppendLine("      <div class=\"nqb-condition-actions\">");
        html.AppendLine($"        <button type=\"submit\" formmethod=\"post\" formaction=\"?handler=RemoveCondition&index={index}\" class=\"nqb-button nqb-button-small nqb-button-text nqb-button-danger\">");
        html.AppendLine("          Remove");
        html.AppendLine("        </button>");
        html.AppendLine("      </div>");

        html.AppendLine("    </div>");
        html.AppendLine("  </div>");
    }

    private void RenderLogicalOperatorSelect(StringBuilder html, int index, LogicalOperator currentOperator)
    {
        html.AppendLine("  <div class=\"nqb-logical-operator\">");
        html.AppendLine($"    <select name=\"Conditions[{index}].LogicalOperator\" class=\"nqb-select nqb-select-small\">");
        html.AppendLine($"      <option value=\"And\"{(currentOperator == LogicalOperator.And ? " selected" : "")}>AND</option>");
        html.AppendLine($"      <option value=\"Or\"{(currentOperator == LogicalOperator.Or ? " selected" : "")}>OR</option>");
        html.AppendLine("    </select>");
        html.AppendLine("  </div>");
    }
}
