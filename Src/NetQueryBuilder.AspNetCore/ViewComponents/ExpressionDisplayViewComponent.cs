using Microsoft.AspNetCore.Mvc;
using NetQueryBuilder.AspNetCore.Services;

namespace NetQueryBuilder.AspNetCore.ViewComponents;

/// <summary>
/// View component for displaying the current query expression
/// </summary>
public class ExpressionDisplayViewComponent : ViewComponent
{
    private readonly IQuerySessionService _sessionService;

    public ExpressionDisplayViewComponent(IQuerySessionService sessionService)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
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
