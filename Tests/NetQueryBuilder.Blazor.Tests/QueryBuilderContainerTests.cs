using System.Reflection;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
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
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("mudBlazor.js");
        _ = RenderComponent<MudTestLayout>();
    }

    [Fact]
    public void QueryBuilderContainer_RendersCorrectly_WithEntities()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert
        Assert.NotNull(cut.Find("div.mud-toolbar"));
        Assert.NotNull(cut.Find("button"));
        Assert.NotNull(cut.Find("div.mud-select"));

        // Vérifie que le bouton New Query est présent
        var newQueryButton = cut.Find("button");
        Assert.Contains("New Query", newQueryButton.TextContent);
    }
    //
    // [Fact]
    // public void QueryBuilderContainer_ShowsAllEntities_InDropdown()
    // {
    //     // Act
    //     var cut = RenderComponent<QueryBuilderContainer>();
    //
    //     // Assert
    //     // Ouvrir le menu déroulant
    //     var select = cut.Find("div.mud-select");
    //     select.Click();
    //
    //     // Vérifier que les deux entités sont présentes dans le menu déroulant
    //     var items = cut.FindAll(".mud-list-item");
    //     Assert.Equal(2, items.Count);
    //     Assert.Contains(items, item => item.TextContent.Contains("TestEntity"));
    //     Assert.Contains(items, item => item.TextContent.Contains("AnotherEntity"));
    // }

    [Fact]
    public void QueryBuilderContainer_SelectsFirstEntity_ByDefault()
    {
        // Act
        var cut = RenderComponent<QueryBuilderContainer>();

        // Assert
        // Vérifier que le QueryBuilder est rendu (cela indique qu'une entité est sélectionnée)
        // Nous ne pouvons pas interroger directement le composant générique, 
        // mais nous pouvons vérifier qu'il n'y a pas de message demandant de sélectionner une entité
        Assert.DoesNotContain("Please select an entity", cut.Markup);
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

        // Act - Simuler un clic sur le bouton "Run Query"
        cut.Find("button").Click();
        cut.WaitForState(() => cut.FindAll("table").Count > 0);
        // Assert
        var renderedComponent = cut.FindComponent<QueryResultTable<TestEntity>>();
        Assert.NotNull(renderedComponent);
    }

    // [Fact]
    // public void EntitySelect_ValueChanged_ShouldUpdateSelectedEntity()
    // {
    //     var cut = RenderComponent<QueryBuilderContainer>();
    //
    //     // Act - Simulation du changement de valeur dans MudSelect
    //     // Nous utilisons InvokeAsync pour appeler directement la méthode qui est liée à l'événement ValueChanged
    //     cut.InvokeAsync(() => cut.Instance.OnEntitySelect(typeof(TestEntity)));
    //
    //     // Assert - Vérifier que l'entité sélectionnée a changé
    //     // Nous pouvons vérifier indirectement que le contenu du composant affiche le nom de la nouvelle entité
    //     Assert.Contains("TestEntity", cut.Markup);
    // }
}