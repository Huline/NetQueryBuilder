using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Configurations;
using System.Text;

namespace NetQueryBuilder.AspNetCore.TagHelpers;

/// <summary>
/// Tag helper for rendering an entity type selector dropdown
/// </summary>
[HtmlTargetElement("nqb-entity-selector")]
public class EntitySelectorTagHelper : TagHelper
{
    private readonly IQuerySessionService _sessionService;
    private readonly IQueryConfigurator _configurator;

    public EntitySelectorTagHelper(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
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

        // Get available entity types
        var entityTypes = _sessionService.GetAvailableEntityTypes(_configurator);
        var state = _sessionService.GetState(SessionId);

        // Change output tag
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"nqb-form-group {CssClass}".Trim());

        // Build HTML
        var html = new StringBuilder();

        html.AppendLine("<label for=\"entity-selector\" class=\"nqb-label\">Select Entity Type</label>");
        html.AppendLine("<div class=\"nqb-input-group\">");
        html.AppendLine("  <select id=\"entity-selector\" name=\"SelectedEntityType\" class=\"nqb-select\">");
        html.AppendLine("    <option value=\"\">-- Select an Entity --</option>");

        foreach (var entityType in entityTypes)
        {
            var typeName = entityType.FullName ?? entityType.Name;
            var displayName = entityType.Name;
            var selected = state.SelectedEntityType == entityType ? " selected" : "";

            html.AppendLine($"    <option value=\"{typeName}\"{selected}>{displayName}</option>");
        }

        html.AppendLine("  </select>");
        html.AppendLine("  <button type=\"submit\" formmethod=\"post\" formaction=\"?handler=ChangeEntity\" class=\"nqb-button nqb-button-primary\">");
        html.AppendLine("    Load Entity");
        html.AppendLine("  </button>");
        html.AppendLine("</div>");

        output.Content.SetHtmlContent(html.ToString());
    }
}
