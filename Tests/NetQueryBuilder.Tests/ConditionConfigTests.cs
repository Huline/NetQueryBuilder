using NetQueryBuilder.Configurations;
using NetQueryBuilder.Tests.Mocks;

namespace NetQueryBuilder.Tests;

public class ConditionConfigTests
{
    [Fact]
    public void ConditionProperties_WhenSomeAreRemoved_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureConditions(s => s.RemoveFields(nameof(Person.FirstName)));
        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName == nameof(Person.FirstName));
        Assert.Null(propertyPath);
    }

    [Fact]
    public void ConditionProperties_WhenSomeAreFiltered_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureConditions(s => s.LimitToFields(nameof(Person.FirstName)));
        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName == nameof(Person.FirstName));
        Assert.NotNull(propertyPath);
        Assert.Single(query.ConditionPropertyPaths);
    }

    [Fact]
    public void ConditionProperties_WhenTypesAreFiltered_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureConditions(s => s.ExcludeRelationships(typeof(Address)));
        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName.Contains("Address"));
        Assert.Null(propertyPath);
    }

    [Fact]
    public void ConditionProperties_WhenDeepthIsLimited_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureConditions(s => s.LimitDepth(0));
        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName.Contains("Address"));
        Assert.Null(propertyPath);
        Assert.Equal(5, query.ConditionPropertyPaths.Count);
    }

    [Fact]
    public void ConditionProperties_WhenStringifierIsSetted_ShouldRenameThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureConditions(s => s.UseStringifier(new TestStringifier()));
        var query = configurator
            .BuildFor<Person>();

        Assert.True(query.ConditionPropertyPaths.All(p => p.DisplayName() == p.PropertyFullName.ToUpper()));
    }
}