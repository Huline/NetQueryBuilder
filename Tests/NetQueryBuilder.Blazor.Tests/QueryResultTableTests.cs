using Bunit;
using NetQueryBuilder.Blazor.Components;
using NetQueryBuilder.Configurations;

namespace NetQueryBuilder.Blazor.Tests;

public class QueryResultTableTests : TestContext
{
    [Fact]
    public void QueryResultTable_RendersCorrectly_WithEmptyData()
    {
        var data = new List<TestEntity>();
        var properties = new List<SelectPropertyPath>();

        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Properties, properties)
        );

        var divs = cut.FindAll("div");
        var notFoundDiv = divs.FirstOrDefault(div => div.TextContent == "No result found");
        Assert.NotNull(notFoundDiv);
    }

    [Fact]
    public void QueryResultTable_RendersCorrectly_WithData()
    {
        var entity = new TestEntity { Name = "Test", Id = 1 };
        var data = new List<TestEntity> { entity };

        var query = new QueryableQueryConfigurator<TestEntity>(data.AsQueryable()).BuildFor<TestEntity>();

        var cut = RenderComponent<QueryResultTable<TestEntity>>(parameters => parameters
            .Add(p => p.Data, data)
            .Add(p => p.Properties, query.SelectPropertyPaths)
        );

        Assert.NotNull(cut.Find("table"));
        Assert.Equal(2, cut.FindAll("th").Count);
        Assert.Equal(2, cut.FindAll("td").Count);
    }
}