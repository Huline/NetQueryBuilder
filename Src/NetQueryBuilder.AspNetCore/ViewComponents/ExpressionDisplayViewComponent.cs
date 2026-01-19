using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NetQueryBuilder.AspNetCore.Services;

namespace NetQueryBuilder.AspNetCore.ViewComponents;

/// <summary>
/// View component for displaying the current query expression
/// </summary>
public class ExpressionDisplayViewComponent : ViewComponent
{
    private readonly IQuerySessionService _sessionService;
    private readonly ILogger<ExpressionDisplayViewComponent> _logger;

    public ExpressionDisplayViewComponent(
        IQuerySessionService sessionService,
        ILogger<ExpressionDisplayViewComponent> logger)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IViewComponentResult Invoke(string sessionId)
    {
        if (string.IsNullOrEmpty(sessionId))
        {
            return Content(string.Empty);
        }

        var state = _sessionService.GetState(sessionId);
        var expression = state.CurrentExpression ?? "No expression generated yet";

        // Must explicitly specify "Default" view name, otherwise the string is interpreted as a view name
        return View("Default", expression);
    }
}
