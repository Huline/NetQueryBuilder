using System.Data.Entity;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NetQueryBuilder.Configurations;
using NetQueryBuilder.EntityFrameworkNet.Tests.Data;
using NetQueryBuilder.EntityFrameworkNet.Tests.Data.Models;
using NetQueryBuilder.EntityFrameworkNet4;
using Xunit;

namespace NetQueryBuilder.EntityFrameworkNet.Tests
{
    public class ConditionConfigTests
    {
        private readonly MyDbContext _dbContext;
        private readonly IQueryConfigurator _queryConfigurator;

        public ConditionConfigTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IQueryConfigurator, EfQueryConfigurator<MyDbContext>>();
            Database.SetInitializer(new TestDbInitializer());
            _dbContext = new MyDbContext();
            serviceCollection.AddScoped<MyDbContext>(_ => _dbContext);
            var provider = serviceCollection.BuildServiceProvider();
            _queryConfigurator = provider.GetRequiredService<IQueryConfigurator>();
        }

        [Fact]
        public void ConditionProperties_WhenSomeAreRemoved_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureConditions(s => s.RemoveFields(nameof(Person.FirstName)));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName == nameof(Person.FirstName));
            Assert.Null(propertyPath);
        }

        [Fact]
        public void ConditionProperties_WhenSomeAreFiltered_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureConditions(s => s.LimitToFields(nameof(Person.FirstName)));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName == nameof(Person.FirstName));
            Assert.NotNull(propertyPath);
            Assert.Single(query.ConditionPropertyPaths);
        }

        [Fact]
        public void ConditionProperties_WhenTypesAreFiltered_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureConditions(s => s.ExcludeRelationships(typeof(Address)));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName.Contains("Address"));
            Assert.Null(propertyPath);
        }

        [Fact]
        public void ConditionProperties_WhenDeepthIsLimited_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureConditions(s => s.LimitDepth(0));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var propertyPath = query.ConditionPropertyPaths.FirstOrDefault(p => p.PropertyFullName.Contains("Address"));
            Assert.Null(propertyPath);
            Assert.Equal(6, query.ConditionPropertyPaths.Count);
        }

        [Fact]
        public void ConditionProperties_WhenStringifierIsSetted_ShouldRenameThem()
        {
            _queryConfigurator.ConfigureConditions(s => s.UseStringifier(new TestStringifier()));
            var query = _queryConfigurator
                .BuildFor<Person>();

            Assert.True(query.ConditionPropertyPaths.All(p => p.DisplayName() == p.PropertyFullName.ToUpper()));
        }
    }
}