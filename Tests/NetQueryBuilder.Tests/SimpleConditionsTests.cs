using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Tests.Mocks;

namespace NetQueryBuilder.Tests;

public class SimpleConditionsTests
{
    [Fact]
    public async Task BuildDefaultQuery_ReturnsAllResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(people.Count(), results.Count);
    }

    [Fact]
    public async Task BuildEqualConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Single(results);
    }

    [Fact]
    public async Task BuildNotEqualConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<NotEqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task BuildGreaterThanConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<GreaterThanOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id"), 1);
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task BuildGreaterOrEqualThanConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<GreaterThanOrEqualOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id"), 1);
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task BuildLessThanConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<LessThanOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id"), 3);
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task BuildLessOrEqualThanConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<LessThanOrEqualOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id"), 3);
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task BuildInListConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id");
        var @operator = propertyPath.GetCompatibleOperators().FirstOrDefault(o => o.ToString() == "In list");
        query.Condition.CreateNew(propertyPath, @operator, new List<int> { 1, 2 });
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task BuildNotInListConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.ConditionPropertyPaths.First(p => p.PropertyFullName == "Id");
        var @operator = propertyPath.GetCompatibleOperators().FirstOrDefault(o => o.ToString() == "Not in list");
        query.Condition.CreateNew(propertyPath, @operator, new List<int> { 1, 2 });
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Single(results);
    }
}