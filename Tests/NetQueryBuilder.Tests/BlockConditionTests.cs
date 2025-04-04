using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Tests.Mocks;

namespace NetQueryBuilder.Tests;

public class BlockConditionsTests
{
    [Fact]
    public async Task OrConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var condition = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
        condition.LogicalOperator = LogicalOperator.Or;
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }


    [Fact]
    public async Task BlockConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
        var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
        conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
        query.Condition.Group(new[] { conditionOneToBlock, conditionTwoToBlock });
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Single(results);
    }

    [Fact]
    public async Task OrBlockConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
        var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
        conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
        var block = query.Condition.Group(new[] { conditionOneToBlock, conditionTwoToBlock })!;
        block.LogicalOperator = LogicalOperator.Or;
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task BlockCondition_WhenLessThan2ConditionsAreSelected_DontGroup()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
        var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
        conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
        var block = query.Condition.Group(new[] { conditionOneToBlock })!;

        var results = await query.Execute() as List<Person>;

        Assert.Null(block);
        Assert.True(query.Condition.Conditions.All(c => c is not BlockCondition));
        Assert.NotNull(results);
        Assert.Single(results);
    }

    [Fact]
    public async Task BuildUnBlockConditionQuery_ReturnsFilteredResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
        var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
        conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
        var block = query.Condition.Group(new[] { conditionOneToBlock, conditionTwoToBlock })!;

        block.Ungroup(new[] { conditionOneToBlock, conditionTwoToBlock });
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(3, query.Condition.Conditions.Count);
        Assert.Single(results);
    }

    [Fact]
    public async Task UngroupOneCondition_WhenOnlyTwoAreAvailable_UngroupTheTwo()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
        var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
        var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
        conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
        var block = query.Condition.Group(new[] { conditionOneToBlock, conditionTwoToBlock })!;

        block.Ungroup(new[] { conditionOneToBlock });
        var results = await query.Execute() as List<Person>;

        Assert.NotNull(results);
        Assert.Equal(3, query.Condition.Conditions.Count);
        Assert.True(query.Condition.Conditions.All(c => c is not BlockCondition));
        Assert.Single(results);
    }
}