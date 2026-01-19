using NetQueryBuilder.AspNetCore.Pages;
using NetQueryBuilder.AspNetCore.Services;
using NetQueryBuilder.Configurations;

namespace NetQueryBuilder.AspNetCoreSampleApp.Pages;

/// <summary>
/// Query builder page using NetQueryBuilder.
/// The base class handles all query building and execution automatically
/// using reflection-based entity dispatch.
/// </summary>
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

    // No need to override OnPostExecuteQueryAsync or OnPostChangePageAsync!
    // The base class uses reflection to automatically dispatch to the correct
    // generic method based on the selected entity type.
}
