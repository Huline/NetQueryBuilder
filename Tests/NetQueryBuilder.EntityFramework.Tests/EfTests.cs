using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFramework.Tests.Data;
using NetQueryBuilder.EntityFramework.Tests.Data.Models;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.EntityFramework.Tests;

public class EfTests
{
    private readonly IQueryConfigurator _queryConfigurator;
    private readonly MyDbContext _dbContext;

    public EfTests()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddDbContext<MyDbContext>(options =>
        {
            options.UseInMemoryDatabase("InMemoryDb");
        });
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
        var results = (await query.Execute()).OfType<object>().ToList();
        
        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }
    
    [Fact]
    public async Task GetAddressFromPersonName()
    {
        var query = _queryConfigurator.BuildFor<Address>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Person.FirstName"), "Alice");
        var results = (await query.Execute()).OfType<object>().ToList();
        
        Assert.NotNull(results);
        Assert.Single(results);
    }
}