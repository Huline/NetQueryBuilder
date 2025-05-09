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
    public class SelectConfigTests
    {
        private readonly MyDbContext _dbContext;
        private readonly IQueryConfigurator _queryConfigurator;

        public SelectConfigTests()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddScoped<IQueryConfigurator, EfQueryConfigurator<MyDbContext>>();
            Database.SetInitializer(new TestDbInitializer());
            _dbContext = new MyDbContext();
            serviceCollection.AddScoped(_ => _dbContext);
            var provider = serviceCollection.BuildServiceProvider();
            _queryConfigurator = provider.GetRequiredService<IQueryConfigurator>();
        }

        [Fact]
        public void SelectProperties_WhenSomeAreRemoved_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureSelect(s => s.RemoveFields(nameof(Person.FirstName)));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var propertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName == nameof(Person.FirstName));
            Assert.Null(propertyPath);
        }

        [Fact]
        public void SelectProperties_WhenSomeAreFiltered_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureSelect(s => s.LimitToFields(nameof(Person.FirstName)));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName == nameof(Person.FirstName));
            Assert.NotNull(selectPropertyPath);
            Assert.Single(query.SelectPropertyPaths);
        }

        [Fact]
        public void SelectProperties_WhenTypesAreFiltered_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureSelect(s => s.ExcludeRelationships(typeof(Address)));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName.Contains("Address"));
            Assert.Null(selectPropertyPath);
        }

        [Fact]
        public void SelectProperties_WhenDeepthIsLimited_ShouldNotShowThem()
        {
            _queryConfigurator.ConfigureSelect(s => s.LimitDepth(0));
            var query = _queryConfigurator
                .BuildFor<Person>();

            var selectPropertyPath = query.SelectPropertyPaths.FirstOrDefault(p => p.Property.PropertyFullName.Contains("Address"));
            Assert.Null(selectPropertyPath);
            Assert.Equal(6, query.SelectPropertyPaths.Count);
        }

        [Fact]
        public void SelectProperties_WhenStringifierIsSetted_ShouldRenameThem()
        {
            _queryConfigurator.ConfigureSelect(s => s.UseStringifier(new TestStringifier()));
            var query = _queryConfigurator
                .BuildFor<Person>();

            Assert.True(query.SelectPropertyPaths.All(p => p.Property.DisplayName() == p.Property.PropertyFullName.ToUpper()));
        }
    }
}