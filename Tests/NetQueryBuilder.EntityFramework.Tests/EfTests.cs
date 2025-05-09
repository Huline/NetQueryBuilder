using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFramework.Tests.Data;
using NetQueryBuilder.EntityFramework.Tests.Data.Models;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.EntityFramework.Tests;

public class EfTests
{
    private readonly MyDbContext _dbContext;
    private readonly IQueryConfigurator _queryConfigurator;

    public EfTests()
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
    public async Task GetAll()
    {
        var query = _queryConfigurator.BuildFor<Person>();
        var results = await query.Execute(50);

        Assert.NotNull(results);
        Assert.Equal(2, results.TotalItems);
    }

    [Fact]
    public async Task GetAddressFromPersonName()
    {
        var query = _queryConfigurator.BuildFor<Address>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Person.FirstName"), "Alice");
        var results = await query.Execute(50);

        Assert.NotNull(results);
        Assert.Single(results.Items);
    }


    [Theory]
    [InlineData("Jones")]
    [InlineData("jones")]
    [InlineData("Jon")]
    [InlineData("ones")]
    [InlineData("on")]
    public async Task LikeOperator(string nameToSearch)
    {
        var query = _queryConfigurator.BuildFor<Person>();
        var lastName = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName");
        query.Condition.CreateNew(lastName, lastName.GetCompatibleOperators().First(o => o.ToString() == "Like"), nameToSearch);
        var results = await query.Execute<Person>(50);

        Assert.NotNull(results);
        Assert.Single(results.Items);
        Assert.True(results.Items.All(r => r.LastName?.Contains(nameToSearch, StringComparison.OrdinalIgnoreCase) ?? false));
    }

    [Theory]
    [InlineData("Jones")]
    [InlineData("jones")]
    [InlineData("Jon")]
    [InlineData("ones")]
    [InlineData("on")]
    public async Task NotLikeOperator(string nameToSearch)
    {
        var query = _queryConfigurator.BuildFor<Person>();
        var lastName = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName");
        query.Condition.CreateNew(lastName, lastName.GetCompatibleOperators().First(o => o.ToString() == "Not like"), nameToSearch);
        var results = await query.Execute<Person>(50);

        Assert.NotNull(results);
        Assert.Single(results.Items);
        Assert.True(results.Items.All(r => !r.LastName?.Contains(nameToSearch, StringComparison.OrdinalIgnoreCase) ?? true));
    }
}