using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Blazor.Components;
using NetQueryBuilder.Configurations;

namespace NetQueryBuilder.Blazor.Tests;

public sealed class QueryBuilderContainerTests : TestContext
{
    public QueryBuilderContainerTests()
    {
        IQueryConfigurator mockQueryConfigurator = new QueryableQueryConfigurator<TestEntity>(new[]
        {
            new TestEntity { Id = 1, Name = "Test" },
            new TestEntity { Id = 2, Name = "Another Test" }
        }.AsQueryable());

        Services.AddSingleton(mockQueryConfigurator);
        JSInterop.Mode = JSRuntimeMode.Loose;
        // Remove MudBlazor specific setup as we no longer use it in our custom components
    }

    [Fact]
    public void QueryBuilderContainer_RendersCorrectly_WithEntities()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert - Check for our new professional design elements
        Assert.NotNull(cut.Find(".nqb-query-builder-container"));
        Assert.NotNull(cut.Find(".nqb-container-header"));
        Assert.NotNull(cut.Find(".nqb-container-title"));
        
        // Verify the New Query button is present
        var newQueryButton = cut.Find("button");
        Assert.Contains("New Query", newQueryButton.TextContent);
        
        // Verify entity selector is present
        Assert.NotNull(cut.Find(".nqb-entity-selector"));
        
        // Verify hero section elements
        Assert.Contains("Query Builder", cut.Markup);
        Assert.Contains("Build and execute dynamic queries", cut.Markup);
    }
    [Fact]
    public void QueryBuilderContainer_ShowsEntitySelectionUI_Correctly()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert
        // Verify the entity selector section is present
        var dataSourceSection = cut.Find(".nqb-container-section");
        Assert.NotNull(dataSourceSection);
        
        // Verify section header and description
        Assert.Contains("Data Source", cut.Markup);
        Assert.Contains("Select the entity type to query from", cut.Markup);
        
        // Verify the select component for entities is present
        var entitySelector = cut.Find(".nqb-entity-selector");
        Assert.NotNull(entitySelector);
    }

    [Fact]
    public void QueryBuilderContainer_SelectsFirstEntity_ByDefault()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert
        // Verify that an entity is selected by checking for query builder sections
        Assert.NotNull(cut.Find(".nqb-query-section"));
        Assert.DoesNotContain("Please select an entity", cut.Markup);
        
        // Verify that entity selection feedback is shown
        Assert.NotNull(cut.Find(".nqb-entity-badge"));
        Assert.Contains("TestEntity", cut.Markup);
    }

    [Fact]
    public void QueryBuilderContainer_CreatesNewQuery_OnButtonClick()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilderContainer>();
        var initialKey = cut.Instance.GetType()
            .GetField("_queryBuilderKey", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(cut.Instance);

        // Act
        var newQueryButton = cut.Find("button");
        newQueryButton.Click();

        // Assert
        var newKey = cut.Instance.GetType()
            .GetField("_queryBuilderKey", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(cut.Instance);

        // Vérifier que la clé a été incrémentée, ce qui force la reconstruction du QueryBuilder
        Assert.NotEqual(initialKey, newKey);
    }

    [Fact]
    public void QueryBuilderContainer_ChangesSelectedEntity_OnDropdownChange()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilderContainer>();

        // Act
        // Simuler la sélection d'une nouvelle entité
        cut.InvokeAsync(() => cut.Instance.OnEntitySelect(typeof(TestEntity)));

        // Assert
        var selectedEntityType = cut.Instance.GetType()
            .GetField("_selectedEntityType", BindingFlags.NonPublic | BindingFlags.Instance)
            ?.GetValue(cut.Instance);

        Assert.Equal(typeof(TestEntity), selectedEntityType);
    }

    [Fact]
    public void RunQueryButton_Click_ShouldExecuteQuery()
    {
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Act - Find and click the "Execute Query" button (our new button text)
        var executeButton = cut.FindAll("button")
            .FirstOrDefault(b => b.TextContent.Contains("Execute Query"));
        Assert.NotNull(executeButton);
        
        executeButton.Click();
        cut.WaitForState(() => cut.FindAll(".nqb-data-table").Count > 0 || 
                               cut.FindAll(".nqb-no-results").Count > 0);
        
        // Assert - Verify results panel is rendered
        var resultsPanel = cut.Find(".nqb-results-panel");
        Assert.NotNull(resultsPanel);
        
        var renderedComponent = cut.FindComponent<QueryResultTable<TestEntity>>();
        Assert.NotNull(renderedComponent);
    }

    [Fact]
    public void QueryBuilderContainer_ShowsEntityBadge_WhenEntitySelected()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert - Verify entity selection is indicated with a badge
        var entityBadge = cut.Find(".nqb-entity-badge");
        Assert.NotNull(entityBadge);
        Assert.Contains("TestEntity", entityBadge.TextContent);
        
        // Verify the success styling for entity selection
        var entityInfo = cut.Find(".nqb-entity-info");
        Assert.NotNull(entityInfo);
        Assert.Contains("Entity selected", cut.Markup);
    }

    [Fact] 
    public void QueryBuilderContainer_HasProfessionalDesign_Elements()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert - Verify professional design elements are present
        Assert.NotNull(cut.Find(".nqb-container-header")); // Gradient header
        Assert.NotNull(cut.Find(".nqb-container-title")); // Professional title
        Assert.NotNull(cut.Find(".nqb-container-icon")); // Header icon
        
        // Verify the main container has proper styling
        var container = cut.Find(".nqb-query-builder-container");
        Assert.NotNull(container);
    }
    
    [Fact]
    public void QueryBuilderContainer_RespondsToEntitySelection_Correctly()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();
        
        // Initially should show TestEntity as selected
        Assert.Contains("TestEntity", cut.Markup);
        Assert.NotNull(cut.Find(".nqb-entity-badge"));
        
        // Verify entity info feedback
        Assert.Contains("Entity selected", cut.Markup);
        var entityInfo = cut.Find(".nqb-entity-info");
        Assert.NotNull(entityInfo);
    }
    
    [Fact]
    public void QueryBuilderContainer_ShowsHeroSection_WithDescription()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();
        
        // Assert - Verify hero section elements
        Assert.Contains("Query Builder", cut.Markup);
        Assert.Contains("Build and execute dynamic queries", cut.Markup);
        
        // Verify professional header structure
        var headerContent = cut.Find(".nqb-container-header-content");
        Assert.NotNull(headerContent);
        
        var titleSection = cut.Find(".nqb-container-title-section");
        Assert.NotNull(titleSection);
    }
    
    [Fact]
    public void QueryBuilderContainer_MaintainsQueryBuilder_AfterEntityChange()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilderContainer>();
        var initialQuerySections = cut.FindAll(".nqb-query-section").Count;
        
        // Act - Change entity selection
        cut.InvokeAsync(() => cut.Instance.OnEntitySelect(typeof(TestEntity)));
        
        // Assert - Query builder should still be present and functional
        var newQuerySections = cut.FindAll(".nqb-query-section");
        Assert.True(newQuerySections.Count >= initialQuerySections);
        
        // Verify query sections are still functional
        Assert.NotNull(cut.Find(".nqb-query-config-panel"));
    }
}