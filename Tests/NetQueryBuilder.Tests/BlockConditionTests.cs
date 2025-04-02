using NetQueryBuilder.Conditions;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.Operators;

namespace NetQueryBuilder.Tests;

public class BlockConditionsTests
{
        [Fact]
        public async Task BuildOrConditionQuery_ReturnsFilteredResults()
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
            Assert.Equal(2,results.Count);
        }
        
        
        [Fact]
        public async Task BuildBlockConditionQuery_ReturnsFilteredResults()
        {
            var people = TestData.GetPeople();
            var configurator = new QueryableQueryConfigurator<Person>(people);
            
            var query = configurator
                .BuildFor<Person>();
            query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
            var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
            var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
            conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
            query.Condition.Group(new []{conditionOneToBlock, conditionTwoToBlock});
            var results = await query.Execute() as List<Person>;
            
            Assert.NotNull(results);
            Assert.Single(results);
        }
        
        [Fact]
        public async Task BuildAnotherBlockConditionQuery_ReturnsFilteredResults()
        {
            var people = TestData.GetPeople();
            var configurator = new QueryableQueryConfigurator<Person>(people);
            
            var query = configurator
                .BuildFor<Person>();
            query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Jean");
            var conditionOneToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "FirstName"), "Marie");
            var conditionTwoToBlock = query.Condition.CreateNew<EqualsOperator>(query.ConditionPropertyPaths.First(p => p.PropertyFullName == "LastName"), "Dupont");
            conditionTwoToBlock.LogicalOperator = LogicalOperator.Or;
            var block= query.Condition.Group(new []{conditionOneToBlock, conditionTwoToBlock})!;
            block.LogicalOperator = LogicalOperator.Or;
            var results = await query.Execute() as List<Person>;
            
            Assert.NotNull(results);
            Assert.Equal(2,results.Count);
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
            var block= query.Condition.Group(new []{conditionOneToBlock, conditionTwoToBlock})!;
            
            block.Ungroup(new []{conditionOneToBlock, conditionTwoToBlock});
            var results = await query.Execute() as List<Person>;
            
            Assert.NotNull(results);
            Assert.Equal(3,query.Condition.Conditions.Count);
            Assert.Equal(1,results.Count);
        }
}
