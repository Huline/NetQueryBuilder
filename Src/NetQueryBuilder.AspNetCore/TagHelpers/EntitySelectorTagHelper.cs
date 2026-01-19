using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<EntitySelectorTagHelper> _logger;

    public EntitySelectorTagHelper(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator,
        ILogger<EntitySelectorTagHelper> logger)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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
            _logger.LogWarning("EntitySelectorTagHelper: SessionId is null or empty, suppressing output");
            output.SuppressOutput();
            return;
        }

        // Get available entity types
        var entityTypes = _sessionService.GetAvailableEntityTypes(_configurator);
        var state = _sessionService.GetState(SessionId);

        _logger.LogDebug("EntitySelectorTagHelper: Rendering {EntityCount} entity types for session {SessionId}",
            entityTypes.Count, SessionId);

        // Change output tag
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"nqb-form-group {CssClass}".Trim());

        // Build HTML
        var html = new StringBuilder();

        html.AppendLine("<label for=\"entity-selector\" class=\"nqb-label\" id=\"entity-selector-label\">Select Entity Type</label>");
        html.AppendLine("<div class=\"nqb-input-group\" role=\"group\" aria-labelledby=\"entity-selector-label\">");
        html.AppendLine("  <select id=\"entity-selector\" name=\"SelectedEntityType\" class=\"nqb-select\" aria-labelledby=\"entity-selector-label\" aria-describedby=\"entity-selector-hint\" aria-required=\"true\">");
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
