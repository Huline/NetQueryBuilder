using Microsoft.AspNetCore.Mvc;
using NetQueryBuilder.AspNetCore.Pages;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.AspNetCoreSampleApp.Models;
using NetQueryBuilder.Configurations;

namespace NetQueryBuilder.AspNetCoreSampleApp.Pages;

public class QueryBuilderModel : NetQueryPageModelBase
{
    public QueryBuilderModel(
        IQuerySessionService sessionService,
        IQueryConfigurator configurator)
        : base(sessionService, configurator)
    {
    }

    public void OnGet()
    {
        // Page loads, state read from session automatically
    }

    public override async Task<IActionResult> OnPostExecuteQueryAsync()
    {
        // Determine the entity type from session state
        var entityType = State.SelectedEntityType;

        if (entityType == null)
        {
            ModelState.AddModelError(string.Empty, "Please select an entity type first.");
            return Page();
        }

        // Execute query based on entity type
        if (entityType == typeof(Person))
        {
            await ExecuteQueryAsync<Person>();
        }
        else if (entityType == typeof(Address))
        {
            await ExecuteQueryAsync<Address>();
        }
        else if (entityType == typeof(Utility))
        {
            await ExecuteQueryAsync<Utility>();
        }
        else
        {
            ModelState.AddModelError(string.Empty, $"Unsupported entity type: {entityType.Name}");
        }

        return Page();
    }

    public override async Task<IActionResult> OnPostChangePageAsync(int page)
    {
        if (page < 1)
            return Page();

        var entityType = State.SelectedEntityType;

        if (entityType == null)
            return Page();

        // Navigate to the specified page based on entity type
        if (entityType == typeof(Person))
        {
            await GoToPageAsync<Person>(page);
        }
        else if (entityType == typeof(Address))
        {
            await GoToPageAsync<Address>(page);
        }
        else if (entityType == typeof(Utility))
        {
            await GoToPageAsync<Utility>(page);
        }

        return Page();
    }
}
