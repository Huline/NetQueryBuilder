using Microsoft.AspNetCore.Mvc;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.AspNetCore.Models;

namespace NetQueryBuilder.AspNetCore.ViewComponents;

/// <summary>
/// View component for displaying pagination controls
/// </summary>
public class PaginationViewComponent : ViewComponent
{
    private readonly IQuerySessionService _sessionService;

    public PaginationViewComponent(IQuerySessionService sessionService)
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

        if (state.Results == null || state.TotalPages == 0)
        {
            return Content(string.Empty);
        }

        var model = new PaginationViewModel
        {
            CurrentPage = state.CurrentPage,
            TotalPages = state.TotalPages,
            TotalItems = state.TotalItems,
            PageSize = state.PageSize
        };

        return View(model);
    }
}

/// <summary>
/// View model for pagination
/// </summary>
public class PaginationViewModel
{
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public int PageSize { get; set; }

    public bool HasPreviousPage => CurrentPage > 1;
    public bool HasNextPage => CurrentPage < TotalPages;
}
