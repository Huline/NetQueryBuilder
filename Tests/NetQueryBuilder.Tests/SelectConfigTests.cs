using NetQueryBuilder.Configurations;
using NetQueryBuilder.Tests.Mocks;

namespace NetQueryBuilder.Tests;

public class SelectConfigTests
{
    [Fact]
    public void SelectProperties_WhenSomeAreRemoved_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureSelect(s => s.RemoveFields(nameof(Person.FirstName)));
        var query = configurator
            .BuildFor<Person>();

        var propertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName == nameof(Person.FirstName));
        Assert.Null(propertyPath);
    }

    [Fact]
    public void SelectProperties_WhenSomeAreFiltered_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureSelect(s => s.LimitToFields(nameof(Person.FirstName)));
        var query = configurator
            .BuildFor<Person>();

        var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName == nameof(Person.FirstName));
        Assert.NotNull(selectPropertyPath);
        Assert.Single(query.SelectPropertyPaths);
    }

    [Fact]
    public void SelectProperties_WhenTypesAreFiltered_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureSelect(s => s.ExcludeRelationships(typeof(Address)));
        var query = configurator
            .BuildFor<Person>();

        var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName.Contains("Address"));
        Assert.Null(selectPropertyPath);
    }

    [Fact]
    public void SelectProperties_WhenDeepthIsLimited_ShouldNotShowThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureSelect(s => s.LimitDepth(0));
        var query = configurator
            .BuildFor<Person>();

        var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName.Contains("Address"));
        Assert.Null(selectPropertyPath);
        Assert.Equal(5, query.SelectPropertyPaths.Count);
    }

    [Fact]
    public void SelectProperties_WhenStringifierIsSetted_ShouldRenameThem()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);
        configurator.ConfigureSelect(s => s.UseStringifier(new TestStringifier()));
        var query = configurator
            .BuildFor<Person>();

        Assert.True(query.SelectPropertyPaths.All(p => p.Property.DisplayName() == p.Property.PropertyFullName.ToUpper()));
    }
}