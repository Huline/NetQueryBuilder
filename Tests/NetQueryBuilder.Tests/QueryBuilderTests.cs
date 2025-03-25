using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;
using NetQueryBuilder.Tests.Mocks;

namespace NetQueryBuilder.Tests;

public class QueryBuilderTests
{
        [Fact]
        public async Task BuildDefaultQuery_ReturnsAllResults()
        {
            var people = TestData.GetPeople();
            var configurator = new QueryableQueryConfigurator<Person>(people);
            
            // Act
            var query = configurator
                .BuildFor<Person>();
                
            var results = await query.Execute() as List<Person>;

            // Assert
            Assert.NotNull(results);
            Assert.Equal(people.Count(), results.Count);
        }
        
        [Fact]
        public async Task BuildConditionQuery_ReturnsFilteredResults()
        {
            var people = TestData.GetPeople();
            var configurator = new QueryableQueryConfigurator<Person>(people);
            
            // Act
            var query = configurator
                .BuildFor<Person>();
            var propertyPath = query.ConditionPropertyPaths.Where(p => p.PropertyFullName == "FirstName").First();
            var simpleCondition = new SimpleCondition(propertyPath, LogicalOperator.And);
            simpleCondition.Operator = propertyPath.GetCompatibleOperators().OfType<EqualsOperator>().First();
            simpleCondition.Value = "Jean";
            (query.Conditions.First() as BlockCondition).Add(simpleCondition);
            var results = await query.Execute() as List<Person>;

            // Assert
            Assert.NotNull(results);
            Assert.Equal(1, results.Count);
        }
}
