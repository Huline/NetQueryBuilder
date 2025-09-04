using NetQueryBuilder.Configurations;
using NetQueryBuilder.Queries;
using NetQueryBuilder.Tests.Mocks;

namespace NetQueryBuilder.Tests;

public class OrderByTests
{
    [Fact]
    public async Task BuildDefaultQuery_ReturnsAllResults()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.OrderBy.Set(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), OrderDirection.Ascending);
        var results = await query.Execute(50);

        Assert.NotNull(results);
        Assert.Equal(people.Count(), results.TotalItems);
        for (var index = 0; index < people.OrderBy(p => p.LastName).ToArray().Length; index++)
        {
            var person = people.OrderBy(p => p.LastName).ToArray()[index];
            var result = results.Items.ElementAt(index);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal(person.LastName, result.LastName);
        }
    }

    [Fact]
    public async Task BuildDefaultQuery_ReturnsAllResults2()
    {
        var people = TestData.GetPeople();
        var configurator = new QueryableQueryConfigurator<Person>(people);

        var query = configurator
            .BuildFor<Person>();
        query.OrderBy.Set(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), OrderDirection.Descending);
        var results = await query.Execute(50);

        Assert.NotNull(results);
        Assert.Equal(people.Count(), results.TotalItems);
        for (var index = 0; index < people.OrderBy(p => p.LastName).ToArray().Length; index++)
        {
            var person = people.OrderBy(p => p.LastName).ToArray()[index];
            var result = results.Items.ElementAt(people.Count() - index - 1);
            Assert.Equal(person.Id, result.Id);
            Assert.Equal(person.LastName, result.LastName);
        }
    }
}