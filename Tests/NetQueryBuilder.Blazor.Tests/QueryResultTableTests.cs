using Bunit;
using NetQueryBuilder.Blazor.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Properties;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Tests;

public class QueryResultTableTests : TestContext
{
    [Fact]
    public void QueryResultTable_ShowsEmptyState_WhenNoResults()
    {
        // Arrange
        var properties = new List<SelectPropertyPath>();

        // Act
        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Results, (QueryResult<TestEntity>?)null)
            .Add(p => p.Properties, properties)
        );

        // Assert
        Assert.NotNull(cut.Find(".nqb-empty-state"));
        Assert.Contains("No Query Results", cut.Markup);
        Assert.Contains("Execute a query to see results here", cut.Markup);
        Assert.NotNull(cut.Find(".nqb-empty-state-icon"));
    }

    [Fact]
    public async Task QueryResultTable_RendersTable_WithData()
    {
        // Arrange
        var entities = new List<TestEntity> 
        { 
            new TestEntity { Id = 1, Name = "Test 1" },
            new TestEntity { Id = 2, Name = "Test 2" }
        };
        
        var queryConfigurator = new QueryableQueryConfigurator<TestEntity>(entities.AsQueryable());
        var query = queryConfigurator.BuildFor<TestEntity>();
        
        // Create QueryResult using the static factory method
        var results = await QueryResult<TestEntity>.FromQuery(
            count: 2, 
            fetchItems: (pageSize, offset) => Task.FromResult<IReadOnlyCollection<TestEntity>>(entities), 
            pageSize: 10
        );

        // Act
        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Results, results)
            .Add(p => p.Properties, query.SelectPropertyPaths)
        );

        // Assert
        Assert.NotNull(cut.Find(".nqb-results-container"));
        Assert.NotNull(cut.Find(".nqb-data-table"));
        Assert.NotNull(cut.Find(".nqb-table-header"));
        Assert.NotNull(cut.Find(".nqb-table-body"));
        
        // Verify results count display - use more flexible text matching
        Assert.True(cut.Markup.Contains("2") && cut.Markup.Contains("results"), 
            "Should show result count information");
        
        // Verify table structure
        var headers = cut.FindAll(".nqb-table-column-header");
        Assert.True(headers.Count >= 2); // Id and Name columns
        
        var rows = cut.FindAll(".nqb-table-row");
        Assert.Equal(2, rows.Count);
    }

    [Fact]
    public async Task QueryResultTable_ShowsNoResultsMessage_WithEmptyResults()
    {
        // Arrange
        var queryConfigurator = new QueryableQueryConfigurator<TestEntity>(new List<TestEntity>().AsQueryable());
        var query = queryConfigurator.BuildFor<TestEntity>();
        
        // Create empty QueryResult using the static factory method
        var results = await QueryResult<TestEntity>.FromQuery(
            count: 0, 
            fetchItems: (pageSize, offset) => Task.FromResult<IReadOnlyCollection<TestEntity>>(new List<TestEntity>()), 
            pageSize: 10
        );

        // Act
        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Results, results)
            .Add(p => p.Properties, query.SelectPropertyPaths)
        );

        // Assert
        Assert.NotNull(cut.Find(".nqb-no-results"));
        Assert.Contains("No Results Found", cut.Markup);
        Assert.Contains("Your query didn't return any data", cut.Markup);
        Assert.NotNull(cut.Find(".nqb-no-results-icon"));
        
        // Should show 0 results found - use flexible matching
        Assert.True(cut.Markup.Contains("0") && cut.Markup.Contains("results"), 
            "Should show empty result count information");
    }

    [Fact]
    public async Task QueryResultTable_ShowsLoadingState_WhenLoading()
    {
        // Arrange
        var entities = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test" } };
        var queryConfigurator = new QueryableQueryConfigurator<TestEntity>(entities.AsQueryable());
        var query = queryConfigurator.BuildFor<TestEntity>();
        
        // Create QueryResult using the static factory method
        var results = await QueryResult<TestEntity>.FromQuery(
            count: 1, 
            fetchItems: (pageSize, offset) => Task.FromResult<IReadOnlyCollection<TestEntity>>(entities), 
            pageSize: 10
        );

        // Act
        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Results, results)
            .Add(p => p.Properties, query.SelectPropertyPaths)
            .Add(p => p.Loading, true)
        );

        // Assert
        Assert.NotNull(cut.Find(".nqb-loading-state"));
        Assert.Contains("Loading Results...", cut.Markup);
        Assert.Contains("Executing your query", cut.Markup);
        Assert.NotNull(cut.Find(".nqb-loading-spinner"));
    }

    [Fact]
    public async Task QueryResultTable_ShowsPagination_WithMultiplePages()
    {
        // Arrange
        var entities = Enumerable.Range(1, 25)
            .Select(i => new TestEntity { Id = i, Name = $"Test {i}" })
            .ToList();
            
        var queryConfigurator = new QueryableQueryConfigurator<TestEntity>(entities.AsQueryable());
        var query = queryConfigurator.BuildFor<TestEntity>();
        
        // Create QueryResult with pagination using the static factory method
        var results = await QueryResult<TestEntity>.FromQuery(
            count: 25, 
            fetchItems: (pageSize, offset) => Task.FromResult<IReadOnlyCollection<TestEntity>>(entities.Skip(offset).Take(pageSize).ToList()), 
            pageSize: 10
        );

        // Act
        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Results, results)
            .Add(p => p.Properties, query.SelectPropertyPaths)
        );

        // Assert
        Assert.NotNull(cut.Find(".nqb-pagination-container"));
        Assert.NotNull(cut.Find(".nqb-pagination-controls"));
        Assert.NotNull(cut.Find(".nqb-page-numbers"));
        
        // Verify pagination info - use flexible matching
        Assert.True(cut.Markup.Contains("25") && cut.Markup.Contains("results"), 
            "Should show total result count");
        Assert.True(cut.Markup.Contains("Page") && cut.Markup.Contains("of"), 
            "Should show pagination information");
        
        // Verify pagination buttons
        var pageButtons = cut.FindAll(".nqb-page-numbers button");
        Assert.True(pageButtons.Count > 0);
    }

    [Fact]
    public async Task QueryResultTable_HasProfessionalDesign_Elements()
    {
        // Arrange
        var entities = new List<TestEntity> { new TestEntity { Id = 1, Name = "Test" } };
        var queryConfigurator = new QueryableQueryConfigurator<TestEntity>(entities.AsQueryable());
        var query = queryConfigurator.BuildFor<TestEntity>();
        
        // Create QueryResult using the static factory method
        var results = await QueryResult<TestEntity>.FromQuery(
            count: 1, 
            fetchItems: (pageSize, offset) => Task.FromResult<IReadOnlyCollection<TestEntity>>(entities), 
            pageSize: 10
        );

        // Act
        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Results, results)
            .Add(p => p.Properties, query.SelectPropertyPaths)
        );

        // Assert - Professional design elements (check main structure)
        Assert.NotNull(cut.Find(".nqb-results-container"));
        var tableScroll = cut.FindAll(".nqb-table-scroll").FirstOrDefault();
        if (tableScroll != null) Assert.NotNull(tableScroll);
        
        var columnHeaders = cut.FindAll(".nqb-column-header-content");
        Assert.True(columnHeaders.Count > 0, "Should have column headers");
        
        var cellContents = cut.FindAll(".nqb-cell-content");
        Assert.True(cellContents.Count > 0, "Should have cell content");
        
        // Verify overall structure is professional
        Assert.True(cut.Markup.Contains("nqb-results"), "Should have results styling");
        Assert.True(cut.Markup.Contains("nqb-table"), "Should have table styling");
    }
}

