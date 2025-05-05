using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFramework.Tests.Data;
using NetQueryBuilder.EntityFramework.Tests.Data.Models;

namespace NetQueryBuilder.EntityFramework.Tests;

public class SelectConfigTests
{
    private readonly MyDbContext _dbContext;
    private readonly IQueryConfigurator _queryConfigurator;

    public SelectConfigTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<MyDbContext>(options => { options.UseInMemoryDatabase("InMemoryDb"); });
        serviceCollection.AddScoped<IQueryConfigurator, EfQueryConfigurator<MyDbContext>>();
        var provider = serviceCollection.BuildServiceProvider();
        _queryConfigurator = provider.GetRequiredService<IQueryConfigurator>();
        _dbContext = provider.GetRequiredService<MyDbContext>();
        _dbContext.SeedDatabase().GetAwaiter().GetResult();
    }

    [Fact]
    public void SelectProperties_WhenSomeAreRemoved_ShouldNotShowThem()
    {
        _queryConfigurator.ConfigureSelect(s => s.RemoveFields(nameof(Person.FirstName)));
        var query = _queryConfigurator
            .BuildFor<Person>();

        var propertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName == nameof(Person.FirstName));
        Assert.Null(propertyPath);
    }

    [Fact]
    public void SelectProperties_WhenSomeAreFiltered_ShouldNotShowThem()
    {
        _queryConfigurator.ConfigureSelect(s => s.LimitToFields(nameof(Person.FirstName)));
        var query = _queryConfigurator
            .BuildFor<Person>();

        var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName == nameof(Person.FirstName));
        Assert.NotNull(selectPropertyPath);
        Assert.Single(query.SelectPropertyPaths);
    }

    [Fact]
    public void SelectProperties_WhenTypesAreFiltered_ShouldNotShowThem()
    {
        _queryConfigurator.ConfigureSelect(s => s.ExcludeRelationships(typeof(Address)));
        var query = _queryConfigurator
            .BuildFor<Person>();

        var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName.Contains("Address"));
        Assert.Null(selectPropertyPath);
    }

    [Fact]
    public void SelectProperties_WhenDeepthIsLimited_ShouldNotShowThem()
    {
        _queryConfigurator.ConfigureSelect(s => s.LimitDepth(0));
        var query = _queryConfigurator
            .BuildFor<Person>();

        var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName.Contains("Address"));
        Assert.Null(selectPropertyPath);
        Assert.Equal(6, query.SelectPropertyPaths.Count);
    }

    [Fact]
    public void SelectProperties_WhenStringifierIsSetted_ShouldRenameThem()
    {
        _queryConfigurator.ConfigureSelect(s => s.UseStringifier(new TestStringifier()));
        var query = _queryConfigurator
            .BuildFor<Person>();

        Assert.True(query.SelectPropertyPaths.All(p => p.Property.DisplayName() == p.Property.PropertyFullName.ToUpper()));
    }

    [Fact]
    public async Task SelectProperties_WhenExecuted_ShouldOnlyThem()
    {
        _queryConfigurator.ConfigureSelect(s => s.RemoveFields(nameof(Person.FirstName)));
        var query = _queryConfigurator
            .BuildFor<Person>();

        var results = await query.Execute<Person>();
        Assert.NotNull(results);
        Assert.All(results, r =>
        {
            var person = r;
            Assert.Null(person.FirstName);
            Assert.NotNull(person.LastName);
        });
    }
}