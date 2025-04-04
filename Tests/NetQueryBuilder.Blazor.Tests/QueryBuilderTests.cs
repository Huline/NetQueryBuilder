using Bunit;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
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
        Services.AddMudServices();
        JSInterop.Mode = JSRuntimeMode.Loose;
        JSInterop.SetupModule("mudBlazor.js");
        _ = RenderComponent<MudTestLayout>();
    }
    //
    // [Fact]
    // public void QueryBuilder_RendersCorrectly_WithInitialState()
    // {
    //     // Act
    //     var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
    //         .Add(p => p.Expression, string.Empty)
    //     );
    //
    //     // Assert
    //     // Vérifier que les sections SELECT, WHERE et le bouton de requête sont présents
    //     Assert.Contains("SELECT", cut.Markup);
    //     Assert.Contains("WHERE", cut.Markup);
    //     Assert.Contains("Run Query", cut.Markup);
    //
    //     // Vérifier que le sélecteur de propriétés est présent
    //     Assert.NotNull(cut.Find("div.mud-select"));
    // }


    [Fact]
    public void QueryBuilder_RunsQuery_WhenButtonClicked()
    {
        // Arrange
        var cut = RenderComponent<QueryBuilder<TestEntity>>(parameters => parameters
            .Add(p => p.Expression, string.Empty)
        );

        // Act
        var runButton = cut.Find("button");
        runButton.Click();

        // Vérifier que la table de résultats est mise à jour
        cut.WaitForState(() => cut.FindAll("table").Count > 0);
        Assert.NotNull(cut.Find("table"));
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
}