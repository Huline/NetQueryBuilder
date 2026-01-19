using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Logging;
using NetQueryBuilder.AspNetCore.Services;
using System.Text;

namespace NetQueryBuilder.AspNetCore.TagHelpers;

/// <summary>
/// Tag helper for rendering property selection checkboxes (SELECT clause)
/// </summary>
[HtmlTargetElement("nqb-property-selector")]
public class PropertySelectorTagHelper : TagHelper
{
    private readonly IQuerySessionService _sessionService;
    private readonly ILogger<PropertySelectorTagHelper> _logger;

    public PropertySelectorTagHelper(
        IQuerySessionService sessionService,
        ILogger<PropertySelectorTagHelper> logger)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
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

        // Get available properties
        var properties = state.Query.SelectPropertyPaths.ToList();

        if (!properties.Any())
        {
            output.SuppressOutput();
            return;
        }

        // Change output tag
        output.TagName = "div";
        output.Attributes.SetAttribute("class", $"nqb-property-selector {CssClass}".Trim());

        // Build HTML
        var html = new StringBuilder();

        html.AppendLine("<h3 class=\"nqb-section-title\" id=\"property-selector-title\">SELECT Properties</h3>");
        html.AppendLine("<p class=\"nqb-hint\" id=\"property-selector-hint\">Choose which properties to display in the results</p>");
        html.AppendLine("<div class=\"nqb-checkbox-group\" role=\"group\" aria-labelledby=\"property-selector-title\" aria-describedby=\"property-selector-hint\">");

        foreach (var selectProperty in properties)
        {
            var property = selectProperty.Property;
            var isChecked = selectProperty.IsSelected ? " checked" : "";
            var propertyId = $"prop-{property.PropertyFullName.Replace(".", "-")}";

            html.AppendLine($"  <label class=\"nqb-checkbox-label\" for=\"{propertyId}\">");
            html.AppendLine($"    <input type=\"checkbox\" ");
            html.AppendLine($"           id=\"{propertyId}\" ");
            html.AppendLine($"           name=\"SelectedProperties\" ");
            html.AppendLine($"           value=\"{property.PropertyFullName}\" ");
            html.AppendLine($"           aria-label=\"Select {property.PropertyName} property\" ");
            html.AppendLine($"           {isChecked} />");

            // Display name with depth indicator
            if (property.HasDeepth)
            {
                // Show nested structure: "Address > City"
                var parts = property.PropertyFullName.Split('.');
                html.AppendLine($"    <span class=\"nqb-property-path\">{string.Join(" > ", parts)}</span>");
            }
            else
            {
                html.AppendLine($"    <span class=\"nqb-property-name\">{property.PropertyName}</span>");
            }

            html.AppendLine("  </label>");
        }

        html.AppendLine("</div>");

        // Add update button
        html.AppendLine("<div class=\"nqb-form-actions\">");
        html.AppendLine("  <button type=\"submit\" formmethod=\"post\" formaction=\"?handler=UpdateProperties\" class=\"nqb-button nqb-button-outlined nqb-button-small\">");
        html.AppendLine("    Update Selection");
        html.AppendLine("  </button>");
        html.AppendLine("</div>");

        output.Content.SetHtmlContent(html.ToString());
    }
}
