using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Blazor.Components;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;

namespace NetQueryBuilder.Blazor.Tests;

public sealed class QueryBuilderTests : TestContext
{
    private readonly IQuery _mockQuery;

    public QueryBuilderTests()
    {
        IQueryConfigurator mockQueryConfigurator =
            // Configuration des mocks
            new QueryableQueryConfigurator<TestEntity>(new[]
            {
                new TestEntity
                {
                    Id = 1,
                    Name = "Test"
                }
            }.AsQueryable());
        _mockQuery = mockQueryConfigurator.BuildFor<TestEntity>();
        Services.AddSingleton(mockQueryConfigurator);
        JSInterop.Mode = JSRuntimeMode.Loose;
        // Removed MudBlazor setup as we use custom components now
    }
    [Fact]
    public void QueryBuilder_RendersCorrectly_WithInitialState()
    {
        // Act
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Assert - Verify our new professional sections are present
        Assert.Contains("SELECT", cut.Markup);
        Assert.Contains("WHERE", cut.Markup);
        Assert.Contains("Execute Query", cut.Markup);

        // Verify the enhanced field selector is present
        var fieldSelectors = cut.FindAll(".nqb-field-selector");
        Assert.True(fieldSelectors.Count > 0);
        
        // Verify section icons are present
        var selectIcons = cut.FindAll(".nqb-section-icon-select");
        var filterIcons = cut.FindAll(".nqb-section-icon-filter");
        Assert.True(selectIcons.Count > 0);
        Assert.True(filterIcons.Count > 0);
        
        // Verify query configuration panel structure
        Assert.NotNull(cut.Find(".nqb-query-config-panel"));
        
        // Verify professional sections
        var querySections = cut.FindAll(".nqb-query-section");
        Assert.True(querySections.Count >= 2); // SELECT and WHERE sections
    }


    [Fact]
    public void QueryBuilder_RunsQuery_WhenButtonClicked()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Act - Find the Execute Query button specifically
        var executeButton = cut.FindAll("button")
            .FirstOrDefault(b => b.TextContent.Contains("Execute Query"));
        Assert.NotNull(executeButton);
        
        executeButton.Click();

        // Verify that the results panel is updated
        cut.WaitForState(() => cut.FindAll(".nqb-data-table").Count > 0 || 
                               cut.FindAll(".nqb-no-results").Count > 0);
        
        // Verify results panel is present (either with data or no results message)
        Assert.NotNull(cut.Find(".nqb-results-panel"));
    }

    // [Fact]
    // public void QueryBuilder_UpdatesExpression_WhenQueryChanges()
    // {
    //     // Arrange
    //     string capturedExpression = null;
    //     var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
    //         .Add(p => p.Expression, string.Empty)
    //     );
    //
    //     // Act - Simuler un changement dans la requête
    //     var eventArgs = new EventArgs();
    //     var handler = _mockQuery.OnChanged;
    //     handler?.Invoke(this, eventArgs);
    //
    //     // Assert
    //     Assert.NotNull(capturedExpression);
    //     // Comme _mockQuery.Compile() retourne Expression.Lambda(Expression.Constant(true))
    //     Assert.Contains("True", capturedExpression);
    // }
    //
    // [Fact]
    // public void QueryBuilder_UpdatesSelectedProperties_WhenSelectionChanges()
    // {
    //     // Arrange
    //     var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
    //         .Add(p => p.Expression, string.Empty)
    //     );
    //
    //     // Obtenir l'instance du composant
    //     var instance = cut.Instance;
    //
    //
    //     // Assert
    //     // Vérifier que la propriété IsSelected a été mise à jour
    //     var selectedProperties = _mockQuery.SelectPropertyPaths
    //         .Where(p => p.IsSelected)
    //         .ToList();
    //
    //     // Si notre implémentation fonctionne correctement, la propriété "Id" devrait être désélectionnée
    //     Assert.Equal(1, selectedProperties.Count); // Seul "Name" devrait rester sélectionné
    // }

    [Fact]
    public void QueryBuilder_CleanupEvent_OnDispose()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );
        // Act
        cut.Dispose();

        // Assert
        // Vérifier que OnChanged a été détaché (difficile à tester directement)
        // Ce test est surtout pour vérifier que DisposeAsync ne génère pas d'exception
        Assert.True(true);
    }
    
    [Fact]
    public void QueryBuilder_ShowsProfessionalSections_WithIcons()
    {
        // Act
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Assert - Verify professional section headers with icons
        var selectIcons = cut.FindAll(".nqb-section-icon-select");
        var filterIcons = cut.FindAll(".nqb-section-icon-filter");
        Assert.True(selectIcons.Count > 0);
        Assert.True(filterIcons.Count > 0);
        
        // Verify section titles and descriptions
        Assert.Contains("Choose fields to include in results", cut.Markup);
        Assert.Contains("Define conditions to filter data", cut.Markup);
    }
    
    [Fact]
    public void QueryBuilder_ShowsFieldSelector_Grid()
    {
        // Act
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Assert - Verify enhanced field selector grid
        var fieldSelectors = cut.FindAll(".nqb-field-selector");
        Assert.True(fieldSelectors.Count > 0);
        
        // Should have field selection UI for TestEntity properties
        var fieldItems = cut.FindAll(".nqb-field-item");
        Assert.True(fieldItems.Count >= 2); // At least Id and Name fields
    }
    
    [Fact]
    public void QueryBuilder_ShowsExpressionPreview_Section()
    {
        // Act
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Assert - Verify expression preview section
        var previewSection = cut.FindAll(".nqb-expression-preview").FirstOrDefault();
        if (previewSection != null)
        {
            // Expression preview should be present when there are conditions
            Assert.Contains("Expression Preview", cut.Markup);
        }
        
        // Verify action buttons section - check for Execute Query button instead
        var executeButton = cut.FindAll("button").FirstOrDefault(b => b.TextContent.Contains("Execute Query"));
        Assert.NotNull(executeButton);
    }
    
    [Fact]
    public void QueryBuilder_HandlesQueryExecution_Gracefully()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Act - Execute query multiple times to test robustness
        var executeButton = cut.FindAll("button")
            .FirstOrDefault(b => b.TextContent.Contains("Execute Query"));
        Assert.NotNull(executeButton);
        
        // First execution
        executeButton.Click();
        cut.WaitForState(() => cut.FindAll(".nqb-data-table").Count > 0 || 
                               cut.FindAll(".nqb-no-results").Count > 0);
        
        // Second execution should not break anything
        executeButton.Click();
        cut.WaitForState(() => cut.FindAll(".nqb-results-panel").Count > 0);
        
        // Assert - Results panel should still be functional
        Assert.NotNull(cut.Find(".nqb-results-panel"));
    }
}